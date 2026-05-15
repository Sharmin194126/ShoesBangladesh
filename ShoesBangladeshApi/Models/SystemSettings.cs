using System;
using System.ComponentModel.DataAnnotations;

namespace ShoesBangladesh.API.Models
{
    public class SystemSettings
    {
        [Key]
        public int Id { get; set; }
        public string EidOfferTitle { get; set; } = "Eid-ul-Adha Special Collection";
        public string EidOfferSubtitle { get; set; } = "Up to 50% Discount on Premium Leather Shoes";
        public int DiscountPercentage { get; set; } = 50;
        public DateTime EidOfferEndTime { get; set; } = DateTime.Now.AddDays(10);
        public bool IsOfferActive { get; set; } = true;
        public string CompanyName { get; set; } = "Shoes Bangladesh";
        public string CompanyDescription { get; set; } = "Your trusted destination for premium fashion and quality footwear in Bangladesh.";
        public string ProductSectionDescription { get; set; } = "Discover our latest and most exclusive footwear collection designed for comfort and style.";
        public string ContactEmail { get; set; } = "info@shoesbangladesh.com";

        public string Phone { get; set; } = "+880 1234 56789";
        public string FacebookPageLink { get; set; } = "https://facebook.com/shoesbangladesh";
        public string? LogoUrl { get; set; } = "/images/logo.png";
        
        public string HeroImageUrl { get; set; } = "/images/hero-shoe.png";
        public string HeroBgImageUrl { get; set; } = "/images/hero-bg.png";

        // New fields for the middle banner section
        public string? BannerImageUrl { get; set; } = "/images/banner-red.png";
        public string? BannerTitle { get; set; } = "Classic Red Series";
        public string? BannerDescription { get; set; } = "Experience ultimate comfort with our premium collection.";
        
        // New fields for the side offer card
        public string? OfferCardTitle { get; set; } = "25%";
        public string? OfferCardSubtitle { get; set; } = "EXTRA OFF";
        public string? OfferCardCouponCode { get; set; } = "Use Code: EID2024";
    }
}
