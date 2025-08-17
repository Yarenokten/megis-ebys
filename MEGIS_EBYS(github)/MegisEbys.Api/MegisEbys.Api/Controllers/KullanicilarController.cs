using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KullanicilarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KullanicilarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Kullanicilar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kullanici>>> GetKullanicilar()
        {
            return await _context.Kullanicilar.Include(k => k.Birim).ToListAsync();
        }

        // GET: api/Kullanicilar/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kullanici>> GetKullanici(int id)
        {
            var kullanici = await _context.Kullanicilar.Include(k => k.Birim).FirstOrDefaultAsync(k => k.Id == id);

            if (kullanici == null)
            {
                return NotFound();
            }

            return kullanici;
        }

        // POST: api/Kullanicilar
        [HttpPost]
        public async Task<ActionResult<Kullanici>> PostKullanici(Kullanici kullanici)
        {
            // Yeni kullanıcı için şifre zorunlu olmalı.
            if (string.IsNullOrEmpty(kullanici.Sifre))
            {
                ModelState.AddModelError("Sifre", "The Sifre field is required.");
                return ValidationProblem(ModelState);
            }

            // NOT: Gerçek bir projede, şifre veritabanına kaydedilmeden önce
            // güvenli bir şekilde hash'lenmelidir.
            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKullanici), new { id = kullanici.Id }, kullanici);
        }

        // PUT: api/Kullanicilar/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKullanici(int id, Kullanici kullaniciDto)
        {
            if (id != kullaniciDto.Id)
            {
                return BadRequest();
            }

            var existingKullanici = await _context.Kullanicilar.FindAsync(id);
            if (existingKullanici == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            // Gelen verilerle mevcut kullanıcıyı güncelle
            existingKullanici.AdSoyad = kullaniciDto.AdSoyad;
            existingKullanici.Eposta = kullaniciDto.Eposta;
            existingKullanici.Yetki = kullaniciDto.Yetki;
            existingKullanici.BirimId = kullaniciDto.BirimId;
            existingKullanici.AktifMi = kullaniciDto.AktifMi;

            // Sadece yeni bir şifre gönderildiyse ve boş değilse güncelle
            if (!string.IsNullOrEmpty(kullaniciDto.Sifre))
            {
                // Burada yeni şifre hash'lenmelidir.
                existingKullanici.Sifre = kullaniciDto.Sifre;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Kullanicilar.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Kullanicilar/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKullanici(int id)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(id);
            if (kullanici == null)
            {
                return NotFound();
            }

            kullanici.AktifMi = !kullanici.AktifMi; // Durumu tersine çevir (Aktif ise Pasif, Pasif ise Aktif yap)

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
