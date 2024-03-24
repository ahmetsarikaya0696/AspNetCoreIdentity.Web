using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            if (!ModelState.IsValid) return View(signUpViewModel);

            AppUser newUser = new() { UserName = signUpViewModel.UserName, Email = signUpViewModel.Email };
            IdentityResult identityResult = await _userManager.CreateAsync(newUser, signUpViewModel.Password);

            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Üyelik kayýt iþlemi baþarýyla gerçekleþtirildi.";

                return RedirectToAction(nameof(SignUp));
            }

            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
