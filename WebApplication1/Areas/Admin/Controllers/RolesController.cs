using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
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

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null) throw new Exception("Belirtilen ID'ye sahip rol bulunamadı!");

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var errorMessages = new List<string>();
                errorMessages.AddRange(result.Errors.Select(error => error.Description));
                TempData["ErrorMessages"] = errorMessages;
            }
            else
            {
                TempData["SuccessMessage"] = "Rol başarıyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);

            if (currentUser == null) throw new Exception("Belirtilen ID'ye sahip kullanıcı bulunamadı!");

            ViewBag.UserId = id;

            var roles = await _roleManager.Roles.ToListAsync();

            var assignRoleToUserViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Exist = await _userManager.IsInRoleAsync(currentUser, role.Name)
                };

                assignRoleToUserViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(assignRoleToUserViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> assignRoleToUserViewModelList)
        {
            List<string> errorMessages = new();
            var currentUser = await _userManager.FindByIdAsync(userId);

            foreach (var assignRoleToUserViewModel in assignRoleToUserViewModelList)
            {
                bool isInRole = await _userManager.IsInRoleAsync(currentUser, assignRoleToUserViewModel.Name);
                if (assignRoleToUserViewModel.Exist && !isInRole)
                {
                    var result = await _userManager.AddToRoleAsync(currentUser, assignRoleToUserViewModel.Name);

                    if (!result.Succeeded) errorMessages.AddRange(result.Errors.Select(x => x.Description));

                }
                else if (!assignRoleToUserViewModel.Exist && isInRole)
                {
                    var result = await _userManager.RemoveFromRoleAsync(currentUser, assignRoleToUserViewModel.Name);

                    if (!result.Succeeded) errorMessages.AddRange(result.Errors.Select(x => x.Description));
                }
            }

            TempData["ErrorMessages"] = errorMessages;

            if (errorMessages.Count == 0) TempData["SuccessMessage"] = "Yetki atama işlemi başarıyla gerçekleştirildi!";


            return View(assignRoleToUserViewModelList);
        }
    }
}
