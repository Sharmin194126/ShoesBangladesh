using System.Collections.Generic;

namespace ShoesBangladesh.API.ViewModels
{
    public class CategoryProductsViewModel
    {
        public List<CategoryProductItem> Products { get; set; } = new();
        public List<CategoryItem> Categories { get; set; } = new();
        public List<ProductTypeItem> ProductTypes { get; set; } = new();

        public int? SelectedCategory { get; set; }
        public int? SelectedType { get; set; }
        public string? SearchQuery { get; set; }
        public string? PriceRange { get; set; }
        public string? SelectedSize { get; set; }

        public class CategoryProductItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal RegularPrice { get; set; }
            public decimal? DiscountPrice { get; set; }
            public int StockQty { get; set; }
            public double AverageRating { get; set; }
            public string Status { get; set; } = "Active";
            public string? AvailableSizes { get; set; }
            public int CategoryId { get; set; }
            public int ProductTypeId { get; set; }
        }

        public class CategoryItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        public class ProductTypeItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
