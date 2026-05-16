using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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
        private List<string>? _availableSizes;
        public List<string> AvailableSizes 
        { 
            get => _availableSizes ?? (string.IsNullOrEmpty(AvailableSizesJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(AvailableSizesJson) ?? new List<string>());
            set { _availableSizes = value; AvailableSizesJson = System.Text.Json.JsonSerializer.Serialize(value); }
        }

        public string SizeQuantitiesJson { get; set; } = "{}";
        private Dictionary<string, int>? _sizeQuantities;
        public Dictionary<string, int> SizeQuantities 
        { 
            get => _sizeQuantities ?? (string.IsNullOrEmpty(SizeQuantitiesJson) ? new Dictionary<string, int>() : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(SizeQuantitiesJson) ?? new Dictionary<string, int>());
            set { _sizeQuantities = value; SizeQuantitiesJson = System.Text.Json.JsonSerializer.Serialize(value); }
        }

        public bool IsEidOffer { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsApproved { get; set; }

        public string Status { get; set; } = "Active";
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public int? DisplaySectionId { get; set; }
        public int? AssignedEmployeeId { get; set; }
    }
}
