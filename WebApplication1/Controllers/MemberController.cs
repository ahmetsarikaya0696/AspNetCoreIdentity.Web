using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            UserInfoViewModel userInfoViewModel = new() { Id = currentUser.Id, UserName = currentUser.UserName, Email = currentUser.Email, Picture = currentUser.Picture };
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
                PictureStr = user.Picture
            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserEditViewModel userEditViewModel)
        {
            if (!ModelState.IsValid) return View();

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            currentUser.UserName = userEditViewModel.UserName;
            currentUser.Email = userEditViewModel.Email;
            currentUser.City = userEditViewModel.City;
            currentUser.Gender = userEditViewModel.Gender;
            currentUser.Birthday = userEditViewModel.Birthday;

            if (userEditViewModel.Picture != null && userEditViewModel.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var fileExtension = Path.GetExtension(userEditViewModel.Picture.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(wwwrootFolder.First(x => x.Name == "user-images").PhysicalPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);

                await userEditViewModel.Picture.CopyToAsync(stream);

                currentUser.Picture = fileName;
                userEditViewModel.PictureStr = currentUser.Picture;
            }

            var identityResult = await _userManager.UpdateAsync(currentUser);

            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Bilgileriniz başarıyla güncellendi";
                await _userManager.UpdateSecurityStampAsync(currentUser);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(currentUser, true);

                ViewBag.Genders = new SelectList(Enum.GetNames(typeof(Gender)));
                return View(userEditViewModel);
            }

            foreach (var error in identityResult.Errors)
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
            var userClaims = User.Claims.Select(x => new ClaimViewModel() { Type = x.Type, Value = x.Value, Issuer = x.Issuer }).ToList();
            return View(userClaims);
        }
    }
}
