using System.ComponentModel.DataAnnotations;

namespace ShoesBangladesh.API.Models
{
    public class HomePageSettings
    {
        [Key]
        public int Id { get; set; }

        public string MarqueeText { get; set; } = "🚚 সারা বাংলাদেশে ক্যাশ অন ডেলিভারি সুবিধা! ⚡ দ্রুত ডেলিভারি, নিশ্চিন্ত শপিং! 👟 সেরা মানের জুতার কালেকশন! ✨ এক্সক্লুসিভ অফার পেতে এখনই অর্ডার করুন!";
        public string HeroHeadline { get; set; } = "বাংলাদেশের সেরা জুতার কালেকশন";
        public string HeroSubtitle { get; set; } = "প্রিমিয়াম কোয়ালিটি, সেরা দাম";
        public string HeroBtnText { get; set; } = "এখনই কিনুন";
        public string HeroBtnLink { get; set; } = "#";
        
        public string QualityTitle { get; set; } = "PREMIUM QUALITY ASSURED";
        public string QualityDescription { get; set; } = "Cash on Delivery across Bangladesh with the fastest delivery service. We ensure that every pair of shoes meets our highest standards of comfort, durability, and style.";

        public bool IsActive { get; set; } = true;
    }
}
