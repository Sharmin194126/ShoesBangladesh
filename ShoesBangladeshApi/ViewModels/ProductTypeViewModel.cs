using System;
using System.ComponentModel.DataAnnotations;

namespace ShoesBangladesh.API.ViewModels
{
    public class ProductTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public bool IsApproved { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
