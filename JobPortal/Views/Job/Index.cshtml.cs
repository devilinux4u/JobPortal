using Microsoft.AspNetCore.Mvc.RazorPages;
using JobPortal.Data;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Views.Job
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IEnumerable<JobPortal.Models.Job> Jobs { get; set; } = new List<JobPortal.Models.Job>();

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            Jobs = _context.Jobs.ToList();
            System.Diagnostics.Debug.WriteLine($"Loaded {Jobs.Count()} jobs: {string.Join(", ", Jobs.Select(j => j.Id))}");
        }
    }
}