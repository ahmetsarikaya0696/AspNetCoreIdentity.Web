using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.Permissions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.Seeds
{
    public class PermissionSeed
    {
        public static async Task SeedAsync(RoleManager<AppRole> roleManager)
        {
            bool hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");

            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

                AppRole basicRole = await roleManager.FindByNameAsync("BasicRole");

                await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Stock.Read));
                await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Order.Read));
                await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Catalog.Read));
            }
        }
    }
}
