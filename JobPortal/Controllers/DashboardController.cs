using JobPortal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
[Authorize(Policy = "CompanyOnly")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var applications = _context.Applications
            .Include(a => a.Job)
            .Include(a => a.User)
            .Where(a => a.Job.CompanyId == companyId)
            .ToList();
        return View(applications);
    }
}