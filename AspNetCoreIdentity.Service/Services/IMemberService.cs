using AspNetCoreIdentity.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AspNetCoreIdentity.Service.Services
{
    public interface IMemberService
    {
        Task<UserInfoViewModel> GetUserInfoByUserNameAsync(string userName);
        Task LogoutAsync();
        Task<bool> CheckPasswordAsync(string userName, string password);
        Task<Tuple<bool, List<IdentityError>>> ChangePasswordAsync(string userName, string oldPassword, string newPassword);
        Task<UserEditViewModel> GetUserEditViewModelByUserNameAsync(string userName);
        SelectList GetGenderSelectlist();
        Task<Tuple<bool, List<IdentityError>>> EditUserAsync(UserEditViewModel userEditViewModel, string userName);
        List<ClaimViewModel> GetClaimViewModels(IEnumerable<Claim> claims);
    }
}
