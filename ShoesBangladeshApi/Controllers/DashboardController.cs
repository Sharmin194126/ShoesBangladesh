using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using ShoesBangladesh.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ShoesBangladesh.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Stats")]
        public async Task<ActionResult<AdminStatsViewModel>> GetStats()
        {
            var stats = new AdminStatsViewModel();

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            var users = await _context.Users.ToListAsync();
            var categories = await _context.Categories.ToListAsync();
            var employees = await _context.Employees.ToListAsync();

            stats.TotalOrders = orders.Count;
            stats.TotalDelivered = orders.Count(o => o.Status == "Delivered");
            stats.TotalPending = orders.Count(o => o.Status == "Pending");
            stats.TotalProcessing = orders.Count(o => o.Status == "Processing");
            stats.TotalShipped = orders.Count(o => o.Status == "Shipped");
            stats.TotalCancelled = orders.Count(o => o.Status == "Cancelled");

            var deliveredOrders = orders.Where(o => o.Status == "Delivered" || o.PaymentStatus == "Paid").ToList();
            stats.TotalRevenue = deliveredOrders.Sum(o => o.TotalAmount);
            stats.PaidOrders = orders.Count(o => o.PaymentStatus == "Paid");
            stats.PendingPaymentOrders = orders.Count(o => o.PaymentStatus == "Pending");

            // Weekly Revenue
            var last7DaysStart = DateTime.UtcNow.Date.AddDays(-6);
            stats.WeeklyRevenue = deliveredOrders.Where(o => o.OrderDate.Date >= last7DaysStart).Sum(o => o.TotalAmount);

            // Profit Approximation (Revenue - VAT)
            var totalVat = orders.Where(o => o.Status == "Delivered" || o.PaymentStatus == "Paid")
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.VatAmount * od.Quantity);
            stats.TotalProfit = stats.TotalRevenue - totalVat;
            stats.TotalExpenses = totalVat;

            // Recent Orders
            stats.RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10).Select(o => new RecentOrderViewModel
            {
                OrderId = o.Id,
                CustomerName = users.FirstOrDefault(u => u.Id == o.UserId)?.FullName ?? "Unknown",
                OrderDate = o.OrderDate,
                Status = o.Status,
                Amount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingAddress = o.ShippingAddress
            }).ToList();

            // Employee Performance
            foreach (var emp in employees)
            {
                var ordersAssigned = orders.Where(o => o.AssignedEmployeeId == emp.Id && o.Status == "Delivered").ToList();
                stats.EmployeePerformances.Add(new EmployeePerformanceViewModel
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    TotalSalesValue = ordersAssigned.Sum(o => o.TotalAmount),
                    ProductsSold = ordersAssigned.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity),
                    TotalProductsManaged = _context.Products.Count(p => p.IsFeatured) // Placeholder for managed products
                });
            }

            // Daily Sales (Last 30 Days)
            var last30DaysStart = DateTime.UtcNow.Date.AddDays(-29);
            for (int i = 0; i < 30; i++)
            {
                var date = last30DaysStart.AddDays(i);
                var dayOrders = deliveredOrders.Where(o => o.OrderDate.Date == date).ToList();
                stats.DailySales.Add(new DailySalesViewModel
                {
                    Date = date,
                    Amount = dayOrders.Sum(o => o.TotalAmount),
                    Pieces = dayOrders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }

            // Weekly Sales (Last 7 Days)
            for (int i = 0; i < 7; i++)
            {
                var date = last7DaysStart.AddDays(i);
                var dayOrders = deliveredOrders.Where(o => o.OrderDate.Date == date).ToList();
                stats.WeeklySales.Add(new DailySalesViewModel
                {
                    Date = date,
                    Amount = dayOrders.Sum(o => o.TotalAmount),
                    Pieces = dayOrders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }

            // Monthly History (This Year)
            var currentYear = DateTime.UtcNow.Year;
            for (int m = 1; m <= 12; m++)
            {
                var monthOrders = deliveredOrders.Where(o => o.OrderDate.Year == currentYear && o.OrderDate.Month == m).ToList();
                stats.MonthlyHistory.Add(new MonthlySalesViewModel
                {
                    Year = currentYear,
                    Month = m,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    Amount = monthOrders.Sum(o => o.TotalAmount),
                    Pieces = monthOrders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }

            // Previous Year Sales
            var prevYear = currentYear - 1;
            for (int m = 1; m <= 12; m++)
            {
                var monthOrders = deliveredOrders.Where(o => o.OrderDate.Year == prevYear && o.OrderDate.Month == m).ToList();
                stats.PreviousYearSales.Add(new MonthlySalesViewModel
                {
                    Year = prevYear,
                    Month = m,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    Amount = monthOrders.Sum(o => o.TotalAmount),
                    Pieces = monthOrders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }

            // Product Type Sales (By Category)
            stats.ProductTypeSales = categories.Select(c => new ProductTypeSalesViewModel
            {
                ProductTypeName = c.Name,
                Amount = deliveredOrders.SelectMany(o => o.OrderDetails).Where(od => od.Product?.CategoryId == c.Id).Sum(od => od.PriceWithVat * od.Quantity),
                Pieces = deliveredOrders.SelectMany(o => o.OrderDetails).Where(od => od.Product?.CategoryId == c.Id).Sum(od => od.Quantity)
            }).ToList();

            // Top Products
            stats.TopProducts = deliveredOrders.SelectMany(o => o.OrderDetails)
                .GroupBy(od => new { od.ProductId, od.Product?.Name, CategoryName = od.Product?.Category?.Name })
                .Select(g => new ProductSalesSummaryViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name ?? "Unknown",
                    Category = g.Key.CategoryName ?? "General",
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.PriceWithVat * od.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(6)
                .ToList();

            stats.CurrentYear = currentYear;
            stats.PreviousYear = prevYear;
            stats.SalesGoal = 1000000; // Example goal

            return stats;
        }

        [HttpGet("GetYearlyGoalStats")]
        public async Task<ActionResult> GetYearlyGoalStats(int year)
        {
            var orders = await _context.Orders
                .Where(o => (o.Status == "Delivered" || o.PaymentStatus == "Paid") && o.OrderDate.Year == year)
                .ToListAsync();

            decimal revenue = orders.Sum(o => o.TotalAmount);
            decimal goal = 1000000; // Placeholder, could be from settings
            decimal percent = goal > 0 ? Math.Min(100, (revenue / goal) * 100) : 0;

            return Ok(new
            {
                year = year,
                revenue = revenue,
                goal = goal,
                remaining = Math.Max(0, goal - revenue),
                percent = Math.Round(percent, 1)
            });
        }

        [HttpGet("GetYearlySales")]
        public async Task<ActionResult<List<MonthlySalesViewModel>>> GetYearlySales(int year)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => (o.Status == "Delivered" || o.PaymentStatus == "Paid") && o.OrderDate.Year == year)
                .ToListAsync();

            var result = new List<MonthlySalesViewModel>();
            for (int m = 1; m <= 12; m++)
            {
                var monthOrders = orders.Where(o => o.OrderDate.Month == m).ToList();
                result.Add(new MonthlySalesViewModel
                {
                    Year = year,
                    Month = m,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    Amount = monthOrders.Sum(o => o.TotalAmount),
                    Pieces = monthOrders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }
            return result;
        }

        [HttpGet("OrdersByStatus/{status}")]
        public async Task<ActionResult<List<RecentOrderViewModel>>> GetOrdersByStatus(string status)
        {
            var query = _context.Orders.Include(o => o.OrderDetails).AsQueryable();
            
            if (status != "All")
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            var users = await _context.Users.ToListAsync();

            return orders.Select(o => new RecentOrderViewModel
            {
                OrderId = o.Id,
                CustomerName = users.FirstOrDefault(u => u.Id == o.UserId)?.FullName ?? "Unknown",
                OrderDate = o.OrderDate,
                Status = o.Status,
                Amount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingAddress = o.ShippingAddress
            }).ToList();
        }
        [HttpGet("DisplaySections")]
        public async Task<ActionResult<List<DisplaySectionViewModel>>> GetDisplaySections()
        {
            var sections = await _context.DisplaySections
                .OrderBy(s => s.DisplayOrder)
                .Select(s => new DisplaySectionViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Subtitle = s.Subtitle,
                    SectionType = s.SectionType,
                    IsActive = s.IsActive,
                    DisplayOrder = s.DisplayOrder
                }).ToListAsync();
            return sections;
        }

        [HttpGet("DisplaySections/{id}")]
        public async Task<ActionResult<DisplaySectionViewModel>> GetDisplaySection(int id)
        {
            var section = await _context.DisplaySections.FindAsync(id);
            if (section == null) return NotFound();

            return new DisplaySectionViewModel
            {
                Id = section.Id,
                Title = section.Title,
                Subtitle = section.Subtitle,
                SectionType = section.SectionType,
                IsActive = section.IsActive,
                DisplayOrder = section.DisplayOrder
            };
        }

        [HttpPost("DisplaySections")]
        public async Task<ActionResult<DisplaySectionViewModel>> CreateDisplaySection(DisplaySectionViewModel model)
        {
            var section = new DisplaySection
            {
                Title = model.Title,
                Subtitle = model.Subtitle,
                SectionType = model.SectionType,
                IsActive = model.IsActive,
                DisplayOrder = model.DisplayOrder
            };

            _context.DisplaySections.Add(section);
            await _context.SaveChangesAsync();

            model.Id = section.Id;
            return CreatedAtAction(nameof(GetDisplaySection), new { id = section.Id }, model);
        }

        [HttpPut("DisplaySections/{id}")]
        public async Task<IActionResult> UpdateDisplaySection(int id, DisplaySectionViewModel model)
        {
            if (id != model.Id) return BadRequest();

            var section = await _context.DisplaySections.FindAsync(id);
            if (section == null) return NotFound();

            section.Title = model.Title;
            section.Subtitle = model.Subtitle;
            section.SectionType = model.SectionType;
            section.IsActive = model.IsActive;
            section.DisplayOrder = model.DisplayOrder;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("DisplaySections/{id}")]
        public async Task<IActionResult> DeleteDisplaySection(int id)
        {
            var section = await _context.DisplaySections.FindAsync(id);
            if (section == null) return NotFound();

            _context.DisplaySections.Remove(section);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDTO model)
        {
            var order = await _context.Orders.FindAsync(model.OrderId);
            if (order == null) return NotFound();

            order.Status = model.Status;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = $"Order status updated to {model.Status}" });
        }

        [HttpGet("PaymentHistory")]
        public async Task<ActionResult<List<PaymentHistoryViewModel>>> GetPaymentHistory()
        {
            var history = await _context.PaymentHistories
                .Include(p => p.Order)
                .ThenInclude(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var users = await _context.Users.ToListAsync();

            return history.Select(p => new PaymentHistoryViewModel
            {
                Id = p.Id,
                OrderId = p.OrderId,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                GatewayName = p.GatewayName,
                Status = p.Status,
                CustomerName = p.CustomerName ?? users.FirstOrDefault(u => u.Id == p.Order.UserId)?.FullName,
                CustomerAccount = p.CustomerAccount,
                PaymentDate = p.PaymentDate,
                Order = new OrderSummaryViewModel
                {
                    Id = p.Order.Id,
                    CreatedAt = p.Order.OrderDate,
                    CustomerName = users.FirstOrDefault(u => u.Id == p.Order.UserId)?.FullName ?? "Unknown",
                    OrderDetails = p.Order.OrderDetails.Select(od => new OrderDetailSummaryViewModel
                    {
                        ProductName = od.Product?.Name ?? "Unknown",
                        ProductImageUrl = od.Product?.ImageUrl ?? "",
                        Quantity = od.Quantity,
                        Price = od.PriceWithVat
                    }).ToList()
                }
            }).ToList();
        }
    }

    public class UpdateOrderStatusDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
