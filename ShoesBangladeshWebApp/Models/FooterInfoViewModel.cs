using System;

namespace ShoesBangladesh.Web.Models
{
    public class FooterInfoViewModel
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
