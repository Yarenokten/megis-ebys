using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisKullanicilarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DisKullanicilarController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisKullanici>>> GetDisKullanicilar()
        {
            // DÜZELTME: Artık Kurum bilgisine DisDepartman üzerinden erişiliyor.
            return await _context.DisKullanicilar
                .Include(k => k.DisDepartman.Kurum)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DisKullanici>> GetDisKullanici(int id)
        {
            // DÜZELTME: Artık Kurum bilgisine DisDepartman üzerinden erişiliyor.
            var disKullanici = await _context.DisKullanicilar
                .Include(k => k.DisDepartman.Kurum)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (disKullanici == null)
            {
                return NotFound();
            }

            return disKullanici;
        }

        [HttpPost]
        public async Task<ActionResult<DisKullanici>> PostDisKullanici(DisKullanici disKullanici)
        {
            _context.DisKullanicilar.Add(disKullanici);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDisKullanici), new { id = disKullanici.Id }, disKullanici);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisKullanici(int id, DisKullanici disKullanici)
        {
            if (id != disKullanici.Id)
            {
                return BadRequest();
            }
            _context.Entry(disKullanici).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DisKullanicilar.Any(e => e.Id == id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDisKullanici(int id)
        {
            var disKullanici = await _context.DisKullanicilar.FindAsync(id);
            if (disKullanici == null)
            {
                return NotFound();
            }
            _context.DisKullanicilar.Remove(disKullanici);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}