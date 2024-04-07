using AspNetCoreIdentity.Core.Permissions;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentity.Web.Seeds
{
    public class PermissionSeed
    {
        public static async Task SeedAsync(RoleManager<AppRole> roleManager)
        {
            bool hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            bool hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");
            bool hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            if (!hasBasicRole)
            {
                await AddReadPermission(roleManager, "BasicRole");
            }

            if (!hasAdvancedRole)
            {
                await AddReadPermission(roleManager, "BasicRole");
                await AddUpdateAndCreatePermission(roleManager, "AdvancedRole");
            }

            if (!hasAdminRole)
            {
                await AddReadPermission(roleManager, "BasicRole");
                await AddUpdateAndCreatePermission(roleManager, "AdvancedRole");
                await AddDeletePermission(roleManager, "AdminRole");
            }
        }

        public static async Task AddReadPermission(RoleManager<AppRole> roleManager, string rol)
        {
            await roleManager.CreateAsync(new AppRole() { Name = rol });

            AppRole basicRole = await roleManager.FindByNameAsync(rol);

            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Stock.Read));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Order.Read));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Catalog.Read));
        }

        public static async Task AddUpdateAndCreatePermission(RoleManager<AppRole> roleManager, string rol)
        {
            await roleManager.CreateAsync(new AppRole() { Name = rol });

            AppRole basicRole = await roleManager.FindByNameAsync(rol);

            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Stock.Create));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Order.Create));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Catalog.Create));

            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Stock.Update));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Order.Update));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Catalog.Update));
        }

        public static async Task AddDeletePermission(RoleManager<AppRole> roleManager, string rol)
        {
            await roleManager.CreateAsync(new AppRole() { Name = rol });

            AppRole basicRole = await roleManager.FindByNameAsync(rol);

            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Stock.Delete));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Order.Delete));
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permission.Catalog.Delete));
        }
    }
}
