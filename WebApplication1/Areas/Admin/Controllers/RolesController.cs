using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roleViewModel = await _roleManager.Roles.Select(x => new RoleViewModel() { Id = x.Id, Name = x.Name }).ToListAsync();
            return View(roleViewModel);
        }

        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleViewModel addRoleViewModel)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = addRoleViewModel.Name });

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(result);
            }


            TempData["SuccessMessage"] = "Rol başarıyla eklendi";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) throw new Exception("Belirtilen ID'ye sahip rol bulunamadı!");

            return View(new RoleUpdateViewModel() { Id = role.Id, Name = role.Name });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleUpdateViewModel roleUpdateViewModel)
        {
            var role = await _roleManager.FindByIdAsync(roleUpdateViewModel.Id);

            if (role == null) throw new Exception("Belirtilen ID'ye sahip rol bulunamadı!");

            role.Name = roleUpdateViewModel.Name;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }


            TempData["SuccessMessage"] = "Rol başarıyla güncellendi!";
            return RedirectToAction(nameof(Index));
        }
    }
}
