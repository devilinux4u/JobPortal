using JobPortal.Data;
using JobPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JobPortal.Controllers
{
    [Route("Job/[action]")] // Explicitly define route for clarity
    public class JobController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous] // Allow anonymous access to test, adjust as needed
        public IActionResult GetAll()
        {
            var userId = User.Identity?.IsAuthenticated ?? false ? _userManager.GetUserId(User) : null;
            var jobs = _context.Jobs.AsQueryable();
            if (User.Identity?.IsAuthenticated ?? false && User.IsInRole("Company"))
            {
                jobs = jobs.Where(j => j.CompanyId == userId);
            }
            // Determine role server-side
            string role = "Anonymous";
            if (User.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Company"))
                    role = "Company";
                else if (User.IsInRole("JobSeeker"))
                    role = "JobSeeker";
            }
            var jobList = jobs.Select(j => new
            {
                id = j.Id,
                title = j.Title,
                description = j.Description,
                location = j.Location,
                salary = j.Salary,
                userRole = role // Add role to the response
            }).ToList();
            return new JsonResult(jobList); // Explicitly return JSON
        }

        [Authorize(Roles = "Company")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Create([Bind("Title,Description,Location,Salary")] Job job)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    job.CompanyId = user.Id;
                    _context.Add(job);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(job);
        }

        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            if (job.CompanyId != userId)
            {
                return Forbid();
            }
            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Location,Salary,CompanyId")] Job job)
        {
            if (id != job.Id) return NotFound();
            var userId = _userManager.GetUserId(User);
            if (job.CompanyId != userId)
            {
                return Forbid();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var job = await _context.Jobs.FirstOrDefaultAsync(m => m.Id == id);
            if (job == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            if (job.CompanyId != userId)
            {
                return Forbid();
            }
            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            if (job.CompanyId != userId)
            {
                return Forbid();
            }
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Apply(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null) return NotFound();
            var model = new Application { JobId = id };
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var user = _userManager.GetUserAsync(User).Result;
                model.Name = user?.UserName ?? "";
                model.Email = user?.Email ?? "";
            }
            ViewBag.Job = job;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("JobId,Name,Email,ResumeUrl")] Application application, int id)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Job = await _context.Jobs.FindAsync(id);
                return View(application);
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var user = await _userManager.GetUserAsync(User);
                application.UserId = user?.Id;
            }
            application.JobId = id;
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return RedirectToAction("Applied", new { id = application.Id });
        }

        public IActionResult Applied(int id)
        {
            var application = _context.Applications.Find(id);
            if (application == null) return NotFound();
            var job = _context.Jobs.Find(application.JobId);
            ViewBag.Job = job;
            return View(application);
        }

        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var companyJobs = await _context.Jobs
                .Where(j => j.CompanyId == userId)
                .Select(j => j.Id)
                .ToListAsync();
            var applications = await _context.Applications
                .Where(a => companyJobs.Contains(a.JobId))
                .Include(a => a.Job)
                .ToListAsync();
            return View(applications);
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}