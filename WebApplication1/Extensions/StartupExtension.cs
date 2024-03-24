﻿using AspNetCoreIdentity.Web.CustomValidations;
using AspNetCoreIdentity.Web.Localization;
using AspNetCoreIdentity.Web.Models;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class StartupExtension
    {
        public static void AddIdentityExtension(this IServiceCollection services)
        {
            // Identity
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = false;

                // Lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<AppDbContext>()
              .AddPasswordValidator<PasswordValidator>()
              .AddUserValidator<UserValidator>()
              .AddErrorDescriber<LocalizationIdentityErrorDescriber>();
        }
    }
}
