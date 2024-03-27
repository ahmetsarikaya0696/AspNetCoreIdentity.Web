using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using AspNetCoreIdentity.Web.Services;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View();

            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var user = await _userManager.FindByEmailAsync(signInViewModel.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "E-posta veya þifre yanlýþ!");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, signInViewModel.Password, signInViewModel.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, $"{user.LockoutEnd.Value:dd.MM.yyyy HH:mm} tarihine kadar giriþ yapamayacaksýnýz");

                return View();
            }

            ModelState.AddModelError(string.Empty, "E-posta veya þifre yanlýþ!");
            ModelState.AddModelError(string.Empty, $"Baþarýsýz giriþ sayýsý : {await _userManager.GetAccessFailedCountAsync(user)}");

            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Bu e-posta adresine ait kullanýcý bulunamadý!");
                return View();
            }

            string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            string resetPasswordLink = Url.Action("ResetPassword", "Home", new { userId = user.Id, token = resetPasswordToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmailAsync(resetPasswordLink, user.Email);

            TempData["SuccessMessage"] = "Þifre yenileme linki e-posta adresinize gönderildi";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            var userId = TempData["userId"]?.ToString();
            var token = TempData["token"]?.ToString();

            if (userId == null || token == null) throw new Exception("Bir hata oluþtu!");


            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Belirtilen ID'ye sahip kullanýcý bulunamadý!");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordViewModel.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Þifre baþarýyla sýfýrlandý!";
            }
            else
            {
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, identityError.Description);
                }
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
