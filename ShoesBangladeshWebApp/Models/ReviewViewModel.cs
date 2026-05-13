using System;

namespace ShoesBangladesh.Web.Models
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? CustomerName { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ImageUrls { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
