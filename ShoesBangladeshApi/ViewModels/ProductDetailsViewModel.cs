using System;
using System.Collections.Generic;

namespace ShoesBangladesh.API.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductDetailItem Product { get; set; } = new();
        public List<string> RelatedImages { get; set; } = new();
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingSummary { get; set; } = new();
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public List<SizeStockViewModel> SizeStocks { get; set; } = new();
        public List<ProductViewModel> RelatedProducts { get; set; } = new();

        public class ProductDetailItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string LongDescription { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public string DetailsImageUrl { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal RegularPrice { get; set; }
            public decimal OfferPercentage { get; set; }
            public decimal VatPercentage { get; set; }
            public decimal DeliveryCharge { get; set; }
            public int StockQty { get; set; }
            public int? MaxOrderQty { get; set; }
            public double AverageRating { get; set; }
            public string? AvailableSizes { get; set; } // Comma separated string for the Split(',') in Razor
            public int ProductTypeId { get; set; }
            public ProductTypeInfo? ProductType { get; set; }
        }

        public class ProductTypeInfo
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public class SizeStockViewModel
        {
            public string Size { get; set; } = string.Empty;
            public int StockQty { get; set; }
        }
    }
}
