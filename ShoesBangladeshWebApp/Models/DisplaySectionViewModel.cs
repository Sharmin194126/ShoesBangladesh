using System;

namespace ShoesBangladesh.Web.Models
{
    public class DisplaySectionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string SectionType { get; set; } = "Grid";
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}
