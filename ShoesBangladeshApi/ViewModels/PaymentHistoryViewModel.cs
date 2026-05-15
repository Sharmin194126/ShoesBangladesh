using System;

namespace ShoesBangladesh.API.ViewModels
{
    public class PaymentHistoryViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string GatewayName { get; set; } = string.Empty;
        public string Status { get; set; } = "Success";
        public string? CustomerName { get; set; }
        public string? CustomerAccount { get; set; }
        public DateTime PaymentDate { get; set; }
        public OrderSummaryViewModel? Order { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailSummaryViewModel> OrderDetails { get; set; } = new();
    }

    public class OrderDetailSummaryViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
