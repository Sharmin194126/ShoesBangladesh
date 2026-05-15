using System.ComponentModel.DataAnnotations;

namespace ShoesBangladesh.API.Models
{
    public class HomeBanner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Subtitle { get; set; } = string.Empty;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string LinkUrl { get; set; } = "#";

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
