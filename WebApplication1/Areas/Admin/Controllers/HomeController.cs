﻿using AspNetCoreIdentity.Repository.Models;
using AspNetCoreIdentity.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            List<UserViewModel> users = await _userManager.Users.Select(x => new UserViewModel()
            {
                Id = x.Id,
                Email = x.Email,
                UserName = x.UserName
            }).ToListAsync();

            return View(users);
        }
    }
}
