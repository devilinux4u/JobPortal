using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JobPortal.Models;
using Microsoft.Extensions.Logging;

namespace JobPortal.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            _logger.LogInformation("Logout initiated. Initial authentication state: {IsAuthenticated}", HttpContext.User?.Identity?.IsAuthenticated ?? false);
            _logger.LogInformation("Cookies before logout: {@Cookies}", Request.Cookies.Keys);

            await _signInManager.SignOutAsync();
            _logger.LogInformation("SignOutAsync completed. Authentication state: {IsAuthenticated}", HttpContext.User?.Identity?.IsAuthenticated ?? false);

            var cookieOptions = new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };
            Response.Cookies.Delete(".AspNetCore.Identity.Application", cookieOptions);

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie, cookieOptions);
            }

            return RedirectToPage("/Home/Index");
        }
    }
}