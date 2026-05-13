using System;
using System.ComponentModel.DataAnnotations;

namespace ShoesBangladesh.API.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;
        [StringLength(15)]
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Customer";
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required, StringLength(20)]
        public string Status { get; set; } = "Active";
    }

    public class Employee
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;
        [StringLength(15)]
        public string? PhoneNumber { get; set; }
        [Required, StringLength(50)]
        public string Designation { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; } = DateTime.UtcNow;
        public decimal Salary { get; set; }
        [Required, StringLength(20)]
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
