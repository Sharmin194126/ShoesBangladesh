using System;

namespace ShoesBangladesh.API.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string MessageType { get; set; } = "General"; // General, LiveChat, Email
        public string Status { get; set; } = "Pending";
        public string? Reply { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class FooterInfo
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Details { get; set; }
        public string? AccountNumber { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class PaymentHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public string TransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string GatewayName { get; set; } = string.Empty;
        public string Status { get; set; } = "Success";
        public string? CustomerName { get; set; }
        public string? CustomerAccount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }

    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? ImageUrls { get; set; } // Semicolon separated
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DisplaySection
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string SectionType { get; set; } = "Grid"; // Grid, Slider, Banner
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}
