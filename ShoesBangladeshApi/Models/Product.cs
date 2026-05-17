namespace ShoesBangladesh.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal OfferPercentage { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal DeliveryCharge { get; set; }
        
        public int StockQuantity { get; set; }
        public int MaxOrderLimit { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        public string AdditionalImagesJson { get; set; } = "[]"; // Store JSON
        public string LongDescription { get; set; } = string.Empty;
        public string DetailsImageUrl { get; set; } = string.Empty;
        
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> AdditionalImages
        {
            get => string.IsNullOrEmpty(AdditionalImagesJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(AdditionalImagesJson) ?? new List<string>();
            set => AdditionalImagesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        public string AvailableSizesJson { get; set; } = "[]"; // Store JSON array of strings
        
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> AvailableSizes
        {
            get => string.IsNullOrEmpty(AvailableSizesJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(AvailableSizesJson) ?? new List<string>();
            set => AvailableSizesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        public string SizeQuantitiesJson { get; set; } = "{}"; // Store JSON object of size -> quantity
        
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Dictionary<string, int> SizeQuantities
        {
            get => string.IsNullOrEmpty(SizeQuantitiesJson) ? new Dictionary<string, int>() : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(SizeQuantitiesJson) ?? new Dictionary<string, int>();
            set => SizeQuantitiesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        public bool IsEidOffer { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }
        
        public string Status { get; set; } = "Active";
        public int ProductTypeId { get; set; }
        public ProductType? ProductType { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        
        public int? DisplaySectionId { get; set; }
        public int? AssignedEmployeeId { get; set; }
    }

}
