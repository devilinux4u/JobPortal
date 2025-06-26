using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty; // Foreign key to ApplicationUser
        [Required]
        public string Location { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public ApplicationUser? Company { get; set; } // Navigation property
    }
}