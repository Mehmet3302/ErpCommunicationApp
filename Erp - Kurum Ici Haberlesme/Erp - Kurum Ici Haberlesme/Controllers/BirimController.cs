using Erp___Kurum_Ici_Haberlesme.Data;
using Erp___Kurum_Ici_Haberlesme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Erp___Kurum_Ici_Haberlesme.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BirimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BirimController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var birimler = await _context.Birim
                                .OrderByDescending(b => b.BirimId)
                                .ToListAsync();
            return View(birimler);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Birimler model)
        {
            if (ModelState.IsValid)
            {
                _context.Birim.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var brm = await _context.Birim.FirstOrDefaultAsync(b => b.BirimId == id);
            if (brm == null)
            {
                return NotFound();
            }
            return View(brm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Birimler model)
        {
            if (id != model.BirimId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Birim.Any(b => b.BirimId == model.BirimId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var birim = await _context.Birim.FindAsync(id);
            if (birim == null)
            {
                return NotFound();
            }
            return View(birim);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var birim = await _context.Birim.FindAsync(id);
            if (birim == null)
            {
                return NotFound();
            }

            var altBirimCount = await _context.AltBirim.CountAsync(ab => ab.BirimId == id);
            var personelCount = await _context.Users.CountAsync(u => u.BirimId == id);

            if (altBirimCount > 0 || personelCount > 0)
            {
                // Eğer birime bağlı alt birim veya personel varsa, silme işlemi engellenir ve hata mesajı gösterilir.
                ModelState.AddModelError(string.Empty, "Bu birime bağlı alt birimler veya personeller bulunduğu için silinemez.");
                return View(birim);
            }

            _context.Birim.Remove(birim);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
