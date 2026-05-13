using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesBangladesh.API.Data;
using ShoesBangladesh.API.Models;
using ShoesBangladesh.API.ViewModels;
using System;
using System.Collections.Generic;
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
                .ToListAsync();

            var users = await _context.Users.ToListAsync();
            var customers = await _context.Customers.ToListAsync();
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
            var last7Days = DateTime.UtcNow.Date.AddDays(-6);
            stats.WeeklyRevenue = deliveredOrders.Where(o => o.OrderDate.Date >= last7Days).Sum(o => o.TotalAmount);

            // Profit Approximation (Revenue - VAT)
            var totalVat = await _context.OrderDetails
                .Where(od => od.Order.Status == "Delivered" || od.Order.PaymentStatus == "Paid")
                .SumAsync(od => od.VatAmount * od.Quantity);
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
                    ProductsSold = ordersAssigned.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }

            // Daily Sales (Last 30 Days)
            var last30Days = DateTime.UtcNow.Date.AddDays(-29);
            for (int i = 0; i < 30; i++)
            {
                var date = last30Days.AddDays(i);
                var dayOrders = deliveredOrders.Where(o => o.OrderDate.Date == date).ToList();
                stats.DailySales.Add(new DailySalesViewModel
                {
                    Date = date,
                    Amount = dayOrders.Sum(o => o.TotalAmount),
                    Pieces = dayOrders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }

            // Top Products
            stats.TopProducts = await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.Order.Status == "Delivered" || od.Order.PaymentStatus == "Paid")
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(g => new ProductSalesSummaryViewModel
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.PriceWithVat * od.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(10)
                .ToListAsync();

            stats.CurrentYear = DateTime.UtcNow.Year;
            stats.PreviousYear = stats.CurrentYear - 1;
            stats.SalesGoal = 500000; // Default

            return stats;
        }

        [HttpGet("PaymentHistory")]
        public async Task<ActionResult<List<PaymentHistoryViewModel>>> GetPaymentHistory()
        {
            var history = await _context.PaymentHistories
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return history.Select(p => new PaymentHistoryViewModel
            {
                Id = p.Id,
                OrderId = p.OrderId,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                GatewayName = p.GatewayName,
                Status = p.Status,
                PaymentDate = p.PaymentDate
            }).ToList();
        }

        [HttpGet("Messages")]
        public async Task<ActionResult<List<ContactMessageViewModel>>> GetMessages()
        {
            var messages = await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return messages.Select(m => new ContactMessageViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Subject = m.Subject,
                Message = m.Message,
                MessageType = m.MessageType,
                Status = m.Status,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        [HttpGet("Reviews")]
        public async Task<ActionResult<List<ReviewViewModel>>> GetReviews()
        {
            var reviews = await _context.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReviewViewModel
            {
                Id = r.Id,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        [HttpGet("Footer")]
        public async Task<ActionResult<FooterInfoViewModel>> GetFooter()
        {
            var footer = await _context.FooterInfos.FirstOrDefaultAsync();
            if (footer == null)
            {
                footer = new FooterInfo { Address = "Update Address", ContactNumber = "000" };
                _context.FooterInfos.Add(footer);
                await _context.SaveChangesAsync();
            }
            return new FooterInfoViewModel
            {
                Id = footer.Id,
                Address = footer.Address,
                ContactNumber = footer.ContactNumber,
                Email = footer.Email,
                FacebookUrl = footer.FacebookUrl,
                TwitterUrl = footer.TwitterUrl,
                InstagramUrl = footer.InstagramUrl,
                LastUpdated = footer.LastUpdated
            };
        }

        [HttpPost("Footer")]
        public async Task<ActionResult> UpdateFooter(FooterInfoViewModel model)
        {
            var existing = await _context.FooterInfos.FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.Address = model.Address;
                existing.Email = model.Email;
                existing.ContactNumber = model.ContactNumber;
                existing.FacebookUrl = model.FacebookUrl;
                existing.TwitterUrl = model.TwitterUrl;
                existing.InstagramUrl = model.InstagramUrl;
                existing.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet("DisplaySections")]
        public async Task<ActionResult<List<DisplaySectionViewModel>>> GetDisplaySections()
        {
            var sections = await _context.DisplaySections
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();

            return sections.Select(s => new DisplaySectionViewModel
            {
                Id = s.Id,
                Title = s.Title,
                Subtitle = s.Subtitle,
                SectionType = s.SectionType,
                IsActive = s.IsActive,
                DisplayOrder = s.DisplayOrder
            }).ToList();
        }

        [HttpPost("DisplaySections")]
        public async Task<ActionResult> CreateDisplaySection(DisplaySectionViewModel model)
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
            return Ok();
        }

        [HttpPut("DisplaySections/{id}")]
        public async Task<ActionResult> EditDisplaySection(int id, DisplaySectionViewModel model)
        {
            var existing = await _context.DisplaySections.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Subtitle = model.Subtitle;
            existing.SectionType = model.SectionType;
            existing.IsActive = model.IsActive;
            existing.DisplayOrder = model.DisplayOrder;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Employees")]
        public async Task<ActionResult<List<EmployeeViewModel>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.User)
                .ToListAsync();

            return employees.Select(e => new EmployeeViewModel
            {
                Id = e.Id,
                UserId = e.UserId,
                Name = e.Name,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Designation = e.Designation,
                JoiningDate = e.JoiningDate,
                Salary = e.Salary,
                Status = e.Status
            }).ToList();
        }

        [HttpGet("EmployeeStats")]
        public async Task<ActionResult<List<EmployeePerformanceViewModel>>> GetEmployeeStats()
        {
            var employees = await _context.Employees.ToListAsync();
            var orders = await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
            var stats = new List<EmployeePerformanceViewModel>();

            foreach (var emp in employees)
            {
                var ordersAssigned = orders.Where(o => o.AssignedEmployeeId == emp.Id && o.Status == "Delivered").ToList();
                stats.Add(new EmployeePerformanceViewModel
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    TotalSalesValue = ordersAssigned.Sum(o => o.TotalAmount),
                    ProductsSold = ordersAssigned.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity)
                });
            }
            return stats;
        }

        [HttpGet("OrdersByStatus/{status}")]
        public async Task<ActionResult<List<RecentOrderViewModel>>> GetOrdersByStatus(string status)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new RecentOrderViewModel
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                Amount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingAddress = o.ShippingAddress
            }).ToList();
        }
    }
}
