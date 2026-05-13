using System;
using System.Collections.Generic;

namespace ShoesBangladesh.API.ViewModels
{
    public class AdminStatsDTO
    {
        public int TotalDelivered { get; set; }
        public int TotalPending { get; set; }
        public int TotalProcessing { get; set; }
        public int TotalCancelled { get; set; }
        public int TotalShipped { get; set; }

        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public int PaidOrders { get; set; }
        public int PendingPaymentOrders { get; set; }

        public decimal TotalProfit { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal SalesGoal { get; set; }
        public decimal WeeklyRevenue { get; set; }

        public List<RecentOrderDTO> RecentOrders { get; set; } = new();
        public List<EmployeePerformanceDTO> EmployeePerformances { get; set; } = new();

        public List<DailySalesDTO> DailySales { get; set; } = new();
        public List<DailySalesDTO> WeeklySales { get; set; } = new();
        public List<ProductTypeSalesDTO> ProductTypeSales { get; set; } = new();
        public List<MonthlySalesDTO> MonthlyHistory { get; set; } = new();
        public List<ProductSalesSummaryDTO> TopProducts { get; set; } = new();
        public List<MonthlySalesDTO> PreviousYearSales { get; set; } = new();
        public int CurrentYear { get; set; }
        public int PreviousYear { get; set; }
    }

    public class ProductSalesSummaryDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public string? Category { get; set; }
    }

    public class DailySalesDTO
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int Pieces { get; set; }
    }

    public class ProductTypeSalesDTO
    {
        public string ProductTypeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Pieces { get; set; }
    }

    public class MonthlySalesDTO
    {
        public string MonthName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public int Pieces { get; set; }
    }

    public class RecentOrderDTO
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class EmployeePerformanceDTO
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalProductsManaged { get; set; }
        public decimal TotalStockValue { get; set; }
        public int ProductsSold { get; set; }
        public decimal TotalSalesValue { get; set; }
    }
}
