using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using MegisEbys.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvraklarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public EvraklarController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ... (Mevcut GET metodları aynı kalacak) ...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evrak>>> GetEvraklar()
        {
            return await _context.Evraklar
                .Include(e => e.EvrakTuru)
                .Include(e => e.SorumluBirim)
                .Include(e => e.GonderenKullanici)
                    .ThenInclude(g => g.Birim)
                .Include(e => e.AliciKullanici)
                .Include(e => e.AliciBirim)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Evrak>> GetEvrak(int id)
        {
            var evrak = await _context.Evraklar
                .Include(e => e.EvrakTuru)
                .Include(e => e.SorumluBirim)
                .Include(e => e.GonderenKullanici)
                    .ThenInclude(g => g.Birim)
                .Include(e => e.AliciKullanici)
                .Include(e => e.AliciBirim)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evrak == null)
            {
                return NotFound();
            }

            return evrak;
        }


        [HttpPost]
        public async Task<ActionResult<Evrak>> PostEvrak(Evrak evrak)
        {
            // --- Otomatik Evrak No Oluşturma ---
            var year = DateTime.Now.Year;
            var random = new Random();
            var randomNumber = random.Next(100000, 999999);
            evrak.EvrakNo = $"{year}-{randomNumber}";

            var yapanKullaniciId = evrak.GonderenKullaniciId ?? 1;
            var yapanKullanici = await _context.Kullanicilar.FindAsync(yapanKullaniciId);

            _context.Evraklar.Add(evrak);
            await _context.SaveChangesAsync();

            var islem = new IslemGecmisi
            {
                Aciklama = $"{yapanKullanici?.AdSoyad ?? "Sistem"} tarafından oluşturuldu.",
                IslemTarihi = DateTime.Now,
                EvrakId = evrak.Id,
                KullaniciId = yapanKullaniciId
            };
            _context.Islemler.Add(islem);
            await _context.SaveChangesAsync();

            // --- DÜZELTME: E-POSTA GÖNDERİM KISMI EKLENDİ ---
            try
            {
                if (evrak.DahiliMi && evrak.AliciKullaniciId.HasValue)
                {
                    var alici = await _context.Kullanicilar.FindAsync(evrak.AliciKullaniciId.Value);
                    if (alici != null && !string.IsNullOrEmpty(alici.Eposta))
                    {
                        var subject = $"Yeni Bir Evrak Aldınız: {evrak.Konu}";
                        var message = $"Merhaba {alici.AdSoyad},<br><br>Size yeni bir evrak gönderildi. Lütfen MEGİS EBYS sistemine giriş yaparak kontrol ediniz.<br><br><b>Konu:</b> {evrak.Konu}";
                        await _emailService.SendEmailAsync(alici.Eposta, subject, message);
                    }
                }
            }
            catch (Exception ex)
            {
                // E-posta gönderimi başarısız olursa logla, ama işlemi durdurma
                Console.WriteLine($"Email gönderme hatası: {ex.Message}");
            }
            // --- BİTİŞ ---

            return CreatedAtAction(nameof(GetEvrak), new { id = evrak.Id }, evrak);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvrak(int id, Evrak evrakDto)
        {
            if (id != evrakDto.Id) return BadRequest();

            var existingEvrak = await _context.Evraklar.FindAsync(id);
            if (existingEvrak == null) return NotFound("Güncellenecek evrak bulunamadı.");

            // Varsayılan olarak işlemi yapan kullanıcı ID'si 1 (ilk admin)
            // Gerçek bir sistemde bu bilgi JWT token'dan alınmalıdır.
            var yapanKullaniciId = 1;
            var yapanKullanici = await _context.Kullanicilar.FindAsync(yapanKullaniciId);
            string aciklama = "";

            if (existingEvrak.Durum != evrakDto.Durum)
            {
                if (evrakDto.Durum == EvrakDurum.Arsivlendi)
                    aciklama = $"{yapanKullanici?.AdSoyad ?? "Sistem"} tarafından arşivlendi.";
                else if (evrakDto.Durum == EvrakDurum.Cevaplandi)
                    aciklama = $"{yapanKullanici?.AdSoyad ?? "Sistem"} tarafından cevaplandı.";

                existingEvrak.Durum = evrakDto.Durum;
            }

            if (existingEvrak.SorumluBirimId != evrakDto.SorumluBirimId && evrakDto.SorumluBirimId.HasValue)
            {
                var yeniBirim = await _context.Birimler.FindAsync(evrakDto.SorumluBirimId);
                aciklama = $"{yapanKullanici?.AdSoyad ?? "Sistem"} tarafından '{yeniBirim?.Ad}' birimine yönlendirildi.";
                existingEvrak.SorumluBirimId = evrakDto.SorumluBirimId;
                existingEvrak.Durum = EvrakDurum.Islemde;
            }

            try
            {
                if (!string.IsNullOrEmpty(aciklama))
                {
                    var islem = new IslemGecmisi { Aciklama = aciklama, IslemTarihi = DateTime.Now, EvrakId = id, KullaniciId = yapanKullaniciId };
                    _context.Islemler.Add(islem);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Evraklar.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvrak(int id)
        {
            var evrak = await _context.Evraklar.FindAsync(id);
            if (evrak == null)
            {
                return NotFound();
            }

            _context.Evraklar.Remove(evrak);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
