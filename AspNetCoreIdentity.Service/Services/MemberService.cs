using AspNetCoreIdentity.Core.Models;
using AspNetCoreIdentity.Core.ViewModels;
using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace AspNetCoreIdentity.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task<Tuple<bool, List<IdentityError>>> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            var result = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!result.Succeeded) return Tuple.Create(false, result.Errors.ToList());

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false);

            return Tuple.Create(true, null as List<IdentityError>);
        }

        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);
            return await _userManager.CheckPasswordAsync(currentUser, password);
        }

        public async Task<Tuple<bool, List<IdentityError>>> EditUserAsync(UserEditViewModel userEditViewModel, string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

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

            if (!identityResult.Succeeded) return Tuple.Create(false, identityResult.Errors.ToList());

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            if (userEditViewModel.Birthday.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, [new Claim("Birthday", currentUser.Birthday.Value.ToString())]);
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }

            return Tuple.Create(false, null as List<IdentityError>);
        }

        public List<ClaimViewModel> GetClaimViewModels(IEnumerable<Claim> claims)
        {
            var userClaims = claims.Select(x => new ClaimViewModel() { Type = x.Type, Value = x.Value, Issuer = x.Issuer }).ToList();
            return userClaims;
        }

        public SelectList GetGenderSelectlist()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }

        public async Task<UserEditViewModel> GetUserEditViewModelByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            UserEditViewModel userEditViewModel = new()
            {
                UserName = user.UserName,
                Email = user.Email,
                Birthday = user.Birthday,
                City = user.City,
                Gender = user.Gender,
                PictureStr = user.Picture
            };

            return userEditViewModel;
        }

        public async Task<UserInfoViewModel> GetUserInfoByUserNameAsync(string userName)
        {
            var currentUser = await _userManager.FindByNameAsync(userName);

            UserInfoViewModel userInfoViewModel = new()
            {
                Id = currentUser.Id,
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Picture = currentUser.Picture
            };

            return userInfoViewModel;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
