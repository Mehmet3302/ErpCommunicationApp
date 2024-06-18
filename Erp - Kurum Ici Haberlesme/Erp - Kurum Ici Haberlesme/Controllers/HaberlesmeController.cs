using Erp___Kurum_Ici_Haberlesme.Data;
using Erp___Kurum_Ici_Haberlesme.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static Erp___Kurum_Ici_Haberlesme.Models.Haberlesme;

namespace Erp___Kurum_Ici_Haberlesme.Controllers
{
    public class HaberlesmeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public HaberlesmeController(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Personeller için talep oluşturma
        [ExcludeRoles("Satın Alma Genel Müdürü", "Satın Alma Şube Müdürü", "Satın Alma Personeli", "Muhasebe Genel Müdürü", "Muhasebe Şube Müdürü", "Muhasebe Personeli")]
        public IActionResult TalepOlustur()
        {
            return View();
        }

        [HttpPost]
        [ExcludeRoles("Satın Alma Genel Müdürü", "Satın Alma Şube Müdürü", "Satın Alma Personeli", "Muhasebe Genel Müdürü", "Muhasebe Şube Müdürü", "Muhasebe Personeli")]
        public async Task<IActionResult> TalepOlustur(Haberlesme model)
        {
            var user = await _userManager.GetUserAsync(User);
            model.UserId = user.Id;
            model.OlusturmaTarihi = DateTime.Now;
            model.Durum = Haberlesme.TalepDurumu.Beklemede;
            _context.Haberlesme.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("TalepTakip");
        }

        [ExcludeRoles("Satın Alma Genel Müdürü", "Satın Alma Şube Müdürü", "Satın Alma Personeli", "Muhasebe Genel Müdürü", "Muhasebe Şube Müdürü", "Muhasebe Personeli")]
        public async Task<IActionResult> TalepTakip()
        {
            var user = await _userManager.GetUserAsync(User);
            var talepler = _context.Haberlesme
                .Where(h => h.UserId == user.Id)
                .ToList();
            return View(talepler);
        }

        // Satın alma personeli için gelen taleplerin listesi
        [Authorize(Roles = "Satın Alma Genel Müdürü, Satın Alma Şube Müdürü, Satın Alma Personeli")]
        public IActionResult Talepler(string priority, string sellerSelected, DateTime? date)
        {
            var query = _context.Haberlesme
                .Include(h => h.User)
                .Where(h => h.Durum == Haberlesme.TalepDurumu.Beklemede);

            // Öncelik durumu filtresi
            if (!string.IsNullOrEmpty(priority))
            {
                var priorityEnum = Enum.Parse<Oncelik>(priority);
                query = query.Where(h => h.OncelikDurumu == priorityEnum);
            }

            // Satıcı seçildi mi filtresi
            if (!string.IsNullOrEmpty(sellerSelected))
            {
                var isSellerSelected = bool.Parse(sellerSelected);
                query = query.Where(h => !string.IsNullOrEmpty(h.SaticiBilgileri) == isSellerSelected);
            }

            // Tarih filtresi
            if (date.HasValue)
            {
                query = query.Where(h => h.OlusturmaTarihi.Date == date.Value.Date);
            }

            var talepler = query.OrderByDescending(h => (int)h.OncelikDurumu).ToList();
            return View(talepler);
        }

        [HttpGet]
        [Authorize(Roles = "Satın Alma Genel Müdürü, Satın Alma Şube Müdürü, Satın Alma Personeli")]
        public IActionResult TalepDetay(int id)
        {
            var talep = _context.Haberlesme.Include(h => h.User).FirstOrDefault(h => h.TalebId == id);
            if (talep == null)
            {
                return NotFound();
            }
            return View(talep);
        }

        [HttpPost]
        [Authorize(Roles = "Satın Alma Genel Müdürü, Satın Alma Şube Müdürü, Satın Alma Personeli")]
        public async Task<IActionResult> TalepDetay(Haberlesme model)
        {
            if (ModelState.IsValid)
            {
                var talep = await _context.Haberlesme.FirstOrDefaultAsync(h => h.TalebId == model.TalebId);
                if (talep != null)
                {
                    if (model.SatinAlmaDurumuu == Haberlesme.SatinAlmaDurumu.Red)
                    {
                        talep.Durum = Haberlesme.TalepDurumu.Red;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Talepler");
                    }

                    talep.SatinAlmaDurumuu = model.SatinAlmaDurumuu;
                    talep.SaticiBilgileri = model.SaticiBilgileri;
                    if (model.SatinAlmaDurumuu == Haberlesme.SatinAlmaDurumu.FiyatAlindi && model.OdemeMiktari.HasValue)
                    {
                        talep.FiyatAlmaTarihi = DateTime.Now;
                        talep.OdemeMiktari = model.OdemeMiktari;
                    }

                    if (model.SatinAlmaDurumuu == Haberlesme.SatinAlmaDurumu.Onaylandi)
                    {
                        talep.Durum = Haberlesme.TalepDurumu.Onaylandi;
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Talepler");
                }
            }
            // ModelState geçerli değilse veya talep bulunamadıysa, talep detaylarını tekrar göster
            return View(model);
        }

        [Authorize(Roles = "Satın Alma Genel Müdürü, Satın Alma Şube Müdürü, Satın Alma Personeli")]
        public async Task<IActionResult> OncekiTalepler(int page = 1)
        {
            var pageSize = 10;
            var onaylananTalepler = _context.Haberlesme
                .Where(h => h.Durum == Haberlesme.TalepDurumu.Onaylandi)
                .OrderByDescending(h => h.OlusturmaTarihi);

            var count = await onaylananTalepler.CountAsync();
            var items = await onaylananTalepler.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;

            return View(items);
        }

        // Onaylanan talepleri listeleme
        [Authorize(Roles = "Muhasebe Genel Müdürü, Muhasebe Şube Müdürü, Muhasebe Personeli")]
        public IActionResult OnaylananTalepler()
        {
            var talepler = _context.Haberlesme
                .Where(h => h.SatinAlmaDurumuu == SatinAlmaDurumu.Onaylandi)
                .Include(h => h.User) // Include the User entity for personnel info
                .Select(h => new
                {
                    h.TalebId,
                    h.User,
                    h.Aciklama,
                    h.OdemeMiktari,
                    h.PdfFile,
                    h.SaticiBilgileri // Satıcı bilgilerini dahil edin
                })
                .AsEnumerable() // Veritabanı sorgusunu burada bitiriyoruz
                .Select(h => new Haberlesme // Asıl modelinize dönüştürün
                {
                    TalebId = h.TalebId,
                    User = h.User,
                    Aciklama = h.Aciklama,
                    OdemeMiktari = h.OdemeMiktari,
                    PdfFile = h.PdfFile,
                    SaticiBilgileri = h.SaticiBilgileri
                }).ToList();

            return View(talepler);
        }

        // Ödeme işlemi
        [HttpPost]
        [Authorize(Roles = "Muhasebe Genel Müdürü, Muhasebe Şube Müdürü, Muhasebe Personeli")]
        public async Task<IActionResult> OdemeYap(int id, IFormFile pdfFileUpload)
        {
            var talep = await _context.Haberlesme.FirstOrDefaultAsync(h => h.TalebId == id);
            if (talep != null)
            {
                if (pdfFileUpload != null && pdfFileUpload.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await pdfFileUpload.CopyToAsync(memoryStream);
                        talep.PdfFile = memoryStream.ToArray();
                    }
                }

                var user = await _userManager.GetUserAsync(User);
                talep.OdemeTarihi = DateTime.Now;
                talep.OdemeOnaylandiMi = true;
                talep.OnaylayanPersonel = user.PersonelAdSoyad; // Onaylayan personelin bilgilerini set ediyoruz
                await _context.SaveChangesAsync();
                return RedirectToAction("GecmisOdemeler");
            }
            return NotFound();
        }

        [Authorize(Roles = "Muhasebe Genel Müdürü, Muhasebe Şube Müdürü, Muhasebe Personeli")]
        public async Task<IActionResult> GecmisOdemeler(int page = 1, decimal? minOdemeTutari = null, decimal? maxOdemeTutari = null, DateTime? odenmeTarihi = null, string odemeYapan = null)
        {
            int pageSize = 10;
            var taleplerQuery = _context.Haberlesme
                .Include(h => h.User)
                .Where(h => h.OdemeOnaylandiMi == true);

            if (minOdemeTutari != null)
            {
                taleplerQuery = taleplerQuery.Where(h => h.OdemeMiktari >= minOdemeTutari);
            }

            if (maxOdemeTutari != null)
            {
                taleplerQuery = taleplerQuery.Where(h => h.OdemeMiktari <= maxOdemeTutari);
            }

            if (odenmeTarihi != null)
            {
                taleplerQuery = taleplerQuery.Where(h => h.OdemeTarihi.HasValue && h.OdemeTarihi.Value.Date == odenmeTarihi.Value.Date);
            }

            if (!string.IsNullOrEmpty(odemeYapan))
            {
                taleplerQuery = taleplerQuery.Where(h => h.OnaylayanPersonel.Contains(odemeYapan));
            }

            var totalTalepler = await taleplerQuery.CountAsync();
            var pagedTalepler = await taleplerQuery.OrderByDescending(h => h.OdemeTarihi)
                                                   .Skip((page - 1) * pageSize)
                                                   .Take(pageSize)
                                                   .ToListAsync();

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalTalepler / (double)pageSize);
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < ViewBag.TotalPages;

            return View(pagedTalepler);
        }

        public IActionResult DownloadPdf(int id)
        {
            var talep = _context.Haberlesme.FirstOrDefault(h => h.TalebId == id);
            if (talep == null || talep.PdfFile == null)
            {
                return NotFound();
            }

            return File(talep.PdfFile, "application/pdf", $"Talep_{talep.TalebId}.pdf");
        }

        [Authorize(Roles = "Muhasebe Genel Müdürü, Muhasebe Şube Müdürü, Muhasebe Personeli")]
        public IActionResult DownloadExcel(int id)
        {
            var talep = _context.Haberlesme
                .Include(h => h.User)
                .FirstOrDefault(h => h.TalebId == id);

            if (talep == null)
            {
                return NotFound();
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Talep");

                worksheet.Cells[1, 1].Value = "Talep Id";
                worksheet.Cells[1, 2].Value = "Personel";
                worksheet.Cells[1, 3].Value = "Açıklama";
                worksheet.Cells[1, 4].Value = "Ödeme Miktarı";
                worksheet.Cells[1, 5].Value = "Ödeme Tarihi";
                worksheet.Cells[1, 6].Value = "Onaylayan Personel";
                worksheet.Cells[1, 7].Value = "Satıcı Bilgileri";

                worksheet.Cells[2, 1].Value = talep.TalebId;
                worksheet.Cells[2, 2].Value = talep.User?.PersonelAdSoyad;
                worksheet.Cells[2, 3].Value = talep.Aciklama;
                worksheet.Cells[2, 4].Value = talep.OdemeMiktari;

                // Tarih hücresine format atama
                worksheet.Cells[2, 5].Value = talep.OdemeTarihi;
                worksheet.Cells[2, 5].Style.Numberformat.Format = "dd.MM.yyyy HH:mm:ss";

                worksheet.Cells[2, 6].Value = talep.OnaylayanPersonel;
                worksheet.Cells[2, 7].Value = talep.SaticiBilgileri;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"Talep_{talep.TalebId}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
