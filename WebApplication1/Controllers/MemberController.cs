using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            UserInfoViewModel userInfoViewModel = new() { Id = currentUser.Id, UserName = currentUser.UserName, Email = currentUser.Email };
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

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            bool checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, passwordChangeViewModel.OldPassword);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Mevcut şifre yanlış girildi!");
                return View();
            }

            var result = await _userManager.ChangePasswordAsync(currentUser, passwordChangeViewModel.OldPassword, passwordChangeViewModel.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, passwordChangeViewModel.NewPassword, true, false);

            TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi";

            return View();
        }

        public async Task<IActionResult> EditUser()
        {
            ViewBag.Genders = new SelectList(Enum.GetNames(typeof(Gender)));
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditViewModel userEditViewModel = new()
            {
                UserName = user.UserName,
                Email = user.Email,
                Birthday = user.Birthday,
                City = user.City,
                Gender = user.Gender,
                Picture = user.Picture
            };

            return View(userEditViewModel);
        }
    }
}
