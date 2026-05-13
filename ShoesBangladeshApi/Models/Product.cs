namespace ShoesBangladesh.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> AdditionalImages { get; set; } = new();
        public bool IsEidOffer { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }

}
