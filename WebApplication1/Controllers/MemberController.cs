using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity.Name;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }

        public async Task Logout()
        {
            await _memberService.LogoutAsync();
        }

        public async Task<IActionResult> Index()
        {
            UserInfoViewModel userInfoViewModel = await _memberService.GetUserInfoByUserNameAsync(userName);
            return View(userInfoViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (!ModelState.IsValid) return View();

            bool checkOldPassword = await _memberService.CheckPasswordAsync(userName, passwordChangeViewModel.OldPassword);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Mevcut şifre yanlış girildi!");
                return View();
            }

            var (succeeded, errors) = await _memberService.ChangePasswordAsync(userName, passwordChangeViewModel.OldPassword, passwordChangeViewModel.NewPassword);

            if (!succeeded)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }

            TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi";

            return View();
        }

        public async Task<IActionResult> EditUser()
        {
            ViewBag.Genders = _memberService.GetGenderSelectlist();

            var userEditViewModel = await _memberService.GetUserEditViewModelByUserNameAsync(userName);

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid) return View();


            var (succeeded, errors) = await _memberService.EditUserAsync(userEditViewModel, userName);

            if (succeeded)
            {
                TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellendi";

                ViewBag.Genders = _memberService.GetGenderSelectlist();

                return View(await _memberService.GetUserEditViewModelByUserNameAsync(userName));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(userEditViewModel);
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.Message = "Bu sayfayı görmeye yetkiniz bulunmamaktadır.";
            return View();
        }

        public IActionResult Claims()
        {
            var userClaims = _memberService.GetClaimViewModels(User.Claims);
            return View(userClaims);
        }

        [Authorize(Policy = "AnkaraPolicy")]
        public IActionResult Ankara()
        {
            return View();
        }


        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult Exchange()
        {
            return View();
        }

        [Authorize(Policy = "ViolancePolicy")]
        public IActionResult Violance()
        {
            return View();
        }
    }
}
