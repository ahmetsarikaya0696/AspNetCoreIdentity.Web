using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

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

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            // Kullanýcý üye olduktan 10 gün sonrasýna kadar ücretsiz kullanabileceði bir sayfa için claim ekleme iþlemi
            var exchangeExpireDateClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
            var user = await _userManager.FindByNameAsync(signUpViewModel.UserName);
            var claimIdentityResult = await _userManager.AddClaimAsync(user, exchangeExpireDateClaim);

            if (!claimIdentityResult.Succeeded)
            {
                foreach (var error in claimIdentityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            TempData["SuccessMessage"] = "Üyelik kayýt iþlemi baþarýyla gerçekleþtirildi.";

            return RedirectToAction(nameof(SignUp));
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

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Hesabýnýza giriþ 3 dakikalýðýna askýya alýnmýþtýr! 3 dakika sonra tekrar giriþ yapmayý deneyiniz.");

                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "E-posta veya þifre yanlýþ!");
                ModelState.AddModelError(string.Empty, $"Baþarýsýz giriþ sayýsý : {await _userManager.GetAccessFailedCountAsync(user)}");

                return View();
            }

            if (user.Birthday.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(user, signInViewModel.RememberMe, [new Claim("Birthday", user.Birthday.Value.ToString())]);
            }

            return Redirect(returnUrl);
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
            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));

            string resetPasswordLink = Url.Action("ResetPassword", "Home", new { userId = user.Id, token = encodedToken }, HttpContext.Request.Scheme);

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
            var encodedToken = TempData["token"]?.ToString();

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));

            if (userId == null || encodedToken == null) throw new Exception("Bir hata oluþtu!");


            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Belirtilen ID'ye sahip kullanýcý bulunamadý!");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordViewModel.Password);

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
