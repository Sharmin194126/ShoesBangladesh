using System;
using System.ComponentModel.DataAnnotations;

namespace ShoesBangladesh.API.Models
{
    public class Newsletter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    }
}
