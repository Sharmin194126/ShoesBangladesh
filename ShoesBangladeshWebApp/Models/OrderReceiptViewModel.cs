namespace ShoesBangladeshWebApp.Models
{
    public class OrderReceiptViewModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public CartViewModel Cart { get; set; } = new CartViewModel();
    }
}
