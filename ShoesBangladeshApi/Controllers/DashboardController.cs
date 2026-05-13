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
        public async Task<ActionResult<AdminStatsDTO>> GetStats()
        {
            var stats = new AdminStatsDTO();

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
            stats.RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10).Select(o => new RecentOrderDTO
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
                stats.EmployeePerformances.Add(new EmployeePerformanceDTO
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
                stats.DailySales.Add(new DailySalesDTO
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
                .Select(g => new ProductSalesSummaryDTO
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
        public async Task<ActionResult<List<PaymentHistory>>> GetPaymentHistory()
        {
            return await _context.PaymentHistories
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        [HttpGet("Messages")]
        public async Task<ActionResult<List<ContactMessage>>> GetMessages()
        {
            return await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        [HttpGet("Reviews")]
        public async Task<ActionResult<List<Review>>> GetReviews()
        {
            return await _context.Reviews
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();
        }

        [HttpGet("Footer")]
        public async Task<ActionResult<FooterInfo>> GetFooter()
        {
            var footer = await _context.FooterInfos.FirstOrDefaultAsync();
            if (footer == null)
            {
                footer = new FooterInfo { Address = "Update Address", ContactNumber = "000" };
                _context.FooterInfos.Add(footer);
                await _context.SaveChangesAsync();
            }
            return footer;
        }

        [HttpPost("Footer")]
        public async Task<ActionResult> UpdateFooter(FooterInfo model)
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
        public async Task<ActionResult<List<DisplaySection>>> GetDisplaySections()
        {
            return await _context.DisplaySections
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
        }

        [HttpPost("DisplaySections")]
        public async Task<ActionResult> CreateDisplaySection(DisplaySection section)
        {
            _context.DisplaySections.Add(section);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("DisplaySections/{id}")]
        public async Task<ActionResult> EditDisplaySection(int id, DisplaySection section)
        {
            var existing = await _context.DisplaySections.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = section.Title;
            existing.Subtitle = section.Subtitle;
            existing.SectionType = section.SectionType;
            existing.IsActive = section.IsActive;
            existing.DisplayOrder = section.DisplayOrder;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Employees")]
        public async Task<ActionResult<List<Employee>>> GetEmployees()
        {
            return await _context.Employees
                .Include(e => e.User)
                .ToListAsync();
        }

        [HttpGet("EmployeeStats")]
        public async Task<ActionResult<List<EmployeePerformanceDTO>>> GetEmployeeStats()
        {
            var employees = await _context.Employees.ToListAsync();
            var orders = await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
            var stats = new List<EmployeePerformanceDTO>();

            foreach (var emp in employees)
            {
                var ordersAssigned = orders.Where(o => o.AssignedEmployeeId == emp.Id && o.Status == "Delivered").ToList();
                stats.Add(new EmployeePerformanceDTO
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
        public async Task<ActionResult<List<Order>>> GetOrdersByStatus(string status)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
