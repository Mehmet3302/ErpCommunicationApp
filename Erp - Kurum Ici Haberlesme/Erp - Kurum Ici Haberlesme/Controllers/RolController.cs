using Erp___Kurum_Ici_Haberlesme.Models;
using Erp___Kurum_Ici_Haberlesme.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Erp___Kurum_Ici_Haberlesme.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RolController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
            var roleUserViewModels = new List<RoleUserViewModel>();

            foreach (var role in roles)
            {
                var users = _userManager.GetUsersInRoleAsync(role.Name).Result.ToList();
                var roleUserViewModel = new RoleUserViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Users = users
                };
                roleUserViewModels.Add(roleUserViewModel);
            }

            return View(roleUserViewModels);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AppRole model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] AppRole role)
        {
            if (id != role.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRole = await _roleManager.FindByIdAsync(id);
                    if (existingRole == null)
                    {
                        return NotFound();
                    }

                    existingRole.Name = role.Name;
                    var result = await _roleManager.UpdateAsync(existingRole);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Rol güncelleme işlemi başarısız.");
                        return View(role);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Rol güncelleme işlemi başarısız. Bu kayıt artık mevcut değil.");
                    return View(role);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }
    }
}
