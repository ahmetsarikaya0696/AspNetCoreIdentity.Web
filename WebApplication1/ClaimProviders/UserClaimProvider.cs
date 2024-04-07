using AspNetCoreIdentity.Repository.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.ClaimProviders
{
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var userIdentity = principal.Identity as ClaimsIdentity;

            var currentUser = await _userManager.FindByNameAsync(userIdentity.Name);

            if (string.IsNullOrEmpty(currentUser.City)) return principal;

            if (!principal.HasClaim(x => x.Type == "city"))
            {
                Claim cityClaim = new Claim("city", currentUser.City.ToLower());

                userIdentity.AddClaim(cityClaim);
            }

            return principal;
        }
    }
}
