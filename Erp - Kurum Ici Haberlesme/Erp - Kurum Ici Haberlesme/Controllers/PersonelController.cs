using Erp___Kurum_Ici_Haberlesme.Data;
using Erp___Kurum_Ici_Haberlesme.Models;
using Erp___Kurum_Ici_Haberlesme.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Erp___Kurum_Ici_Haberlesme.Controllers
{
    [Authorize(Roles = "İnsan Kaynakları Genel Müdürü, Bilişim Teknolojileri Genel Müdürü, Satın Alma Genel Müdürü, Muhasebe Genel Müdürü, Satın Alma Şube Müdürü, Bilişim Teknolojileri Şube Müdürü, Muhasebe Şube Müdürü, İnsan Kaynakları Şube Müdürü, Admin")]
    public class PersonelController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<AppRole> _roleManager;
        public PersonelController(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "İnsan Kaynakları Genel Müdürü, Bilişim Teknolojileri Genel Müdürü, Satın Alma Genel Müdürü, Muhasebe Genel Müdürü, Satın Alma Şube Müdürü, Bilişim Teknolojileri Şube Müdürü, Muhasebe Şube Müdürü, İnsan Kaynakları Şube Müdürü, Admin")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            List<AppUser> personeller;
            if (User.IsInRole("Admin"))
            {
                personeller = await _userManager.Users.ToListAsync();
            }
            else
            {
                var altBirimId = user.AltBirimId;
                personeller = await _userManager.Users
                    .Where(u => u.AltBirimId == altBirimId)
                    .ToListAsync();
            }

            return View(personeller);
        }

        [Authorize(Roles = "İnsan Kaynakları Genel Müdürü ,Bilişim Teknolojileri Genel Müdürü, Satın Alma Genel Müdürü, Muhasebe Genel Müdürü, Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateViewModel();
            model.Birimler = GetBirimler();
            model.AltBirimler = new List<SelectListItem>();

            ViewBag.Roles = await _roleManager.Roles
                .Where(role => role.Name != "Admin")
                .Select(role => role.Name)
                .ToListAsync();

            return View(model);
        }

        [Authorize(Roles = "İnsan Kaynakları Genel Müdürü, Bilişim Teknolojileri Genel Müdürü, Satın Alma Genel Müdürü, Muhasebe Genel Müdürü, Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Birimler = GetBirimler();
                model.AltBirimler = new List<SelectListItem>();

                var user = new AppUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    PersonelAdSoyad = model.PersonelAdSoyad,
                    TcKimlikNo = model.TcKimlikNo,
                    PasswordHash = model.Password,
                    BirimId = model.BirimId,
                    AltBirimId = model.AltBirimId,
                    ProfilResmi = await ConvertToByteArrayAsync(model.ProfilResmiDosyasi)
                };

                user.Id = Guid.NewGuid().ToString();
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                    // Email doğrulama tokeni oluşturma
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // Email doğrulama işlemi
                    await _userManager.ConfirmEmailAsync(user, token);

                    return RedirectToAction("Index");
                }

                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "İnsan Kaynakları Genel Müdürü, Bilişim Teknolojileri Genel Müdürü, Satın Alma Genel Müdürü, Muhasebe Genel Müdürü, Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            ViewBag.Roles = await _roleManager.Roles
                .Where(role => role.Name != "Admin")
                .Select(role => role.Name)
                .ToListAsync();

            var viewModel = new EditViewModel
            {
                Id = appUser.Id,
                PersonelAdSoyad = appUser.PersonelAdSoyad,
                TcKimlikNo = appUser.TcKimlikNo,
                BirimId = appUser.BirimId,
                AltBirimId = appUser.AltBirimId,
                Birimler = GetBirimler(),
                AltBirimler = await GetAltBirimlerByBirimAsync(appUser.BirimId),
                SelectedRoles = await _userManager.GetRolesAsync(appUser)
            };

            return View(viewModel);
        }

        [Authorize(Roles = "İnsan Kaynakları Genel Müdürü, Bilişim Teknolojileri Genel Müdürü, Satın Alma Genel Müdürü, Muhasebe Genel Müdürü")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditViewModel viewModel)
        {
            if (string.IsNullOrEmpty(id) || id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByIdAsync(id);
                if (appUser == null)
                {
                    return NotFound();
                }

                appUser.PersonelAdSoyad = viewModel.PersonelAdSoyad;
                appUser.TcKimlikNo = viewModel.TcKimlikNo;
                appUser.BirimId = viewModel.BirimId;
                appUser.AltBirimId = viewModel.AltBirimId;


                if (viewModel.ProfilResmiDosyasi != null)
                {
                    appUser.ProfilResmi = viewModel.ProfilResmiDosyasi;
                }

                var result = await _userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRolesAsync(appUser, await _userManager.GetRolesAsync(appUser));
                    if (viewModel.SelectedRoles != null)
                    {
                        await _userManager.AddToRolesAsync(appUser, viewModel.SelectedRoles);
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Güncelleme sırasında bir hata oluştu.");
                    return View(viewModel);
                }
            }

            viewModel.Birimler = GetBirimler();
            viewModel.AltBirimler = await GetAltBirimlerByBirimAsync(viewModel.BirimId);

            return View(viewModel);
        }

        [Authorize(Roles = "İnsan Kaynakları Genel Müdürü, Bilişim Teknolojileri Genel Müdürü, Satın Alma Genel Müdürü, Muhasebe Genel Müdürü")]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (User.IsInRole("Admin"))
            {
                return Forbid(); // Admin rolündeki kullanıcılar silme işlemi yapamaz
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        private async Task<List<SelectListItem>> GetAltBirimlerByBirimAsync(int birimId)
        {
            var altBirimler = await _context.AltBirim.Where(ab => ab.BirimId == birimId)
                                                      .Select(altBirim => new SelectListItem
                                                      {
                                                          Value = altBirim.AltBirimId.ToString(),
                                                          Text = altBirim.AltBirimAdı
                                                      }).ToListAsync();
            return altBirimler;
        }
        private async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            if (file == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        private List<SelectListItem> GetBirimler()
        {
            var birimler = _context.Birim.ToList();
            var birimlerListesi = birimler.Select(birim => new SelectListItem
            {
                Value = birim.BirimId.ToString(),
                Text = birim.BirimAd
            }).ToList();
            return birimlerListesi;
        }
        public JsonResult GetAltBirimlerByBirim(int birimId)
        {
            var altBirimler = _context.AltBirim.Where(ab => ab.BirimId == birimId).ToList();
            return Json(altBirimler);
        }
    }
}
