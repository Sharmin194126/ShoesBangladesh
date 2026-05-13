using System;

namespace ShoesBangladesh.Web.Models
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
    }
}
