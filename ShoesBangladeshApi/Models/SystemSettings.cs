namespace ShoesBangladesh.API.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public string EidOfferTitle { get; set; } = "Eid Special Offer";
        public string EidOfferSubtitle { get; set; } = "Get the best deals on premium footwear";
        public int DiscountPercentage { get; set; } = 20;
        public DateTime EidOfferEndTime { get; set; } = DateTime.Now.AddDays(7);
        public bool IsOfferActive { get; set; } = true;
        
        // Footer & Contact Info
        public string CompanyName { get; set; } = "Shoes Bangladesh";
        public string ContactEmail { get; set; } = "info@shoesbangladesh.com";
        public string Phone { get; set; } = "+880123456789";
        public string FacebookPageLink { get; set; } = "https://facebook.com/shoesbangladesh";
    }
}
