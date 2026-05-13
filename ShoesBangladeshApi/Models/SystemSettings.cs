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
        public string ContactEmail { get; set; } = "info@shoesbangladesh.com";
        public string Phone { get; set; } = "+880 1234 56789";
        public string FacebookPageLink { get; set; } = "https://facebook.com/shoesbangladesh";
    }
}
