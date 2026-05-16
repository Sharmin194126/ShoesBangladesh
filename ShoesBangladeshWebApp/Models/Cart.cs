namespace ShoesBangladeshWebApp.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatPercentage { get; set; }
        
        // Calculated properties
        public decimal VatAmount => UnitPrice * (VatPercentage / 100);
        public decimal PriceWithVat => UnitPrice + VatAmount;
        public decimal LineTotal => PriceWithVat * Quantity;
    }

    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        
        public decimal SubTotal => Items.Sum(i => i.UnitPrice * i.Quantity);
        public decimal TotalVat => Items.Sum(i => i.VatAmount * i.Quantity);
        public decimal GrandTotal => Items.Sum(i => i.LineTotal);
    }
}
