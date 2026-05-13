using System;

namespace ShoesBangladesh.Web.Models
{
    public class ContactMessageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string MessageType { get; set; } = "General";
        public string Status { get; set; } = "Pending";
        public string? Reply { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
