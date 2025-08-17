using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IzinTalepleriController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IzinTalepleriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/IzinTalepleri
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IzinTalebi>>> GetIzinTalepleri()
        {
            // Tüm izin taleplerini, talep eden kullanıcı bilgisiyle birlikte getirir.
            return await _context.IzinTalepleri
                .Include(t => t.TalepEdenKullanici)
                .OrderByDescending(t => t.TalepTarihi)
                .ToListAsync();
        }

        // POST: api/IzinTalepleri
        [HttpPost]
        public async Task<ActionResult<IzinTalebi>> PostIzinTalebi(IzinTalebi izinTalebi)
        {
            _context.IzinTalepleri.Add(izinTalebi);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIzinTalepleri), new { id = izinTalebi.Id }, izinTalebi);
        }

        // PUT: api/IzinTalepleri/5/onayla
        [HttpPut("{id}/onayla")]
        public async Task<IActionResult> OnaylaIzin(int id)
        {
            return await UpdateIzinDurumu(id, IzinDurum.Onaylandi);
        }

        // PUT: api/IzinTalepleri/5/reddet
        [HttpPut("{id}/reddet")]
        public async Task<IActionResult> ReddetIzin(int id)
        {
            return await UpdateIzinDurumu(id, IzinDurum.Reddedildi);
        }

        private async Task<IActionResult> UpdateIzinDurumu(int id, IzinDurum durum)
        {
            var izinTalebi = await _context.IzinTalepleri.FindAsync(id);
            if (izinTalebi == null)
            {
                return NotFound();
            }

            izinTalebi.Durum = durum;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
