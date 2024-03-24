using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            bool startsWithNumeric = int.TryParse(user.UserName[0].ToString(), out _);

            if (startsWithNumeric)
            {
                errors.Add(new IdentityError() { Code = "StartsWithNumericNotAllowed", Description = "Kullanıcı adı sayısal değer ile başlayamaz!" });
            }

            return Task.FromResult(!errors.Any() ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
