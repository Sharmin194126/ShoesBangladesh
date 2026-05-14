using System.ComponentModel.DataAnnotations;

namespace ShoesBangladesh.API.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Regular Price is required.")]
        [Range(0, 1000000, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Offer percentage must be between 0 and 100.")]
        public decimal OfferPercentage { get; set; }

        [Range(0, 100, ErrorMessage = "VAT percentage must be between 0 and 100.")]
        public decimal VatPercentage { get; set; }

        public decimal DeliveryCharge { get; set; }

        public int StockQuantity { get; set; }

        public int MaxOrderLimit { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public List<string> AdditionalImages { get; set; } = new();

        public string AvailableSizesJson { get; set; } = "[]";
        public List<string> AvailableSizes { get; set; } = new();

        public string SizeQuantitiesJson { get; set; } = "{}";
        public Dictionary<string, int> SizeQuantities { get; set; } = new();

        public bool IsEidOffer { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }

        public string Status { get; set; } = "Active";
        public string ProductType { get; set; } = "Regular";

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int? DisplaySectionId { get; set; }
        public int? AssignedEmployeeId { get; set; }
    }
}
