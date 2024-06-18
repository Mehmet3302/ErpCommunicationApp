using Erp___Kurum_Ici_Haberlesme.Data;
using Erp___Kurum_Ici_Haberlesme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Erp___Kurum_Ici_Haberlesme.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AltBirimController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AltBirimController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var altbirimler = await _context.AltBirim
                                .Include(a => a.Birim)
                                .OrderByDescending(a => a.AltBirimAdı)
                                .ToListAsync();
            return View(altbirimler);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Birimler = new SelectList(await _context.Birim.ToListAsync(), "BirimId", "BirimAd");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AltBirimler model)
        {
            model.Birim = await _context.Birim.FirstOrDefaultAsync(b => b.BirimId == model.BirimId);
            _context.AltBirim.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Birimler = new SelectList(await _context.Birim.ToListAsync(), "BirimId", "BirimAd");
            if (id == null)
            {
                return NotFound();
            }
            var altbrm = await _context.AltBirim.FirstOrDefaultAsync(b => b.AltBirimId == id);
            if (altbrm == null)
            {
                return NotFound();
            }
            return View(altbrm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, AltBirimler model)
        {
            if (id != model.AltBirimId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var altBirimToUpdate = await _context.AltBirim.FirstOrDefaultAsync(b => b.AltBirimId == id);
                    if (altBirimToUpdate == null)
                    {
                        return NotFound();
                    }
                    altBirimToUpdate.AltBirimAdı = model.AltBirimAdı;
                    altBirimToUpdate.BirimId = model.BirimId;
                    _context.Update(altBirimToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AltBirim.Any(b => b.AltBirimId == model.AltBirimId))
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

            var altbirim = await _context.AltBirim.FindAsync(id);
            if (altbirim == null)
            {
                return NotFound();
            }

            return View(altbirim);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromForm] int AltBirimId)
        {
            var altbirim = await _context.AltBirim.FindAsync(AltBirimId);
            if (altbirim == null)
            {
                return NotFound();
            }

            var personelCount = await _context.Users.CountAsync(u => u.AltBirimId == AltBirimId);
            if (personelCount > 0)
            {
                // Alt birimde personel varsa, silme işlemi engellenir ve bir hata mesajı gösterilir.
                ModelState.AddModelError(string.Empty, "Bu alt birimde personel bulunduğu için silinemez.");
                return View(altbirim);
            }

            _context.AltBirim.Remove(altbirim);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
