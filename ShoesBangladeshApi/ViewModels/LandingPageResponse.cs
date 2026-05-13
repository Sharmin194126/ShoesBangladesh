using System;
using System.Collections.Generic;

namespace ShoesBangladesh.API.ViewModels

{
    public class LandingPageResponse
    {
        public SystemSettingsDTO Settings { get; set; } = new();
        public List<ProductDTO> Products { get; set; } = new();
        public List<CategoryDTO> Categories { get; set; } = new();
        public int? TargetProductId { get; set; }
    }


    public class SystemSettingsDTO
    {
        public string EidOfferTitle { get; set; } = string.Empty;
        public string EidOfferSubtitle { get; set; } = string.Empty;
        public int DiscountPercentage { get; set; }
        public DateTime EidOfferEndTime { get; set; }
        public bool IsOfferActive { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyDescription { get; set; } = string.Empty;
        public string ProductSectionDescription { get; set; } = string.Empty;
        public string FacebookPageLink { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public string HeroBgImageUrl { get; set; } = string.Empty;
    }

    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> AdditionalImages { get; set; } = new();
        public bool IsEidOffer { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
