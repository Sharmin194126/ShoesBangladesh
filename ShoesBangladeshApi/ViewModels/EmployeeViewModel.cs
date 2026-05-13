using System;

namespace ShoesBangladesh.API.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Designation { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; } = "Active";
    }
}
