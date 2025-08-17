using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KurumlarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public KurumlarController(ApplicationDbContext context) => _context = context;

        // GET: api/Kurumlar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kurum>>> GetKurumlar()
        {
            return await _context.Kurumlar.OrderBy(k => k.Ad).ToListAsync();
        }

        // GET: api/Kurumlar/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kurum>> GetKurum(int id)
        {
            var kurum = await _context.Kurumlar.FindAsync(id);

            if (kurum == null)
            {
                return NotFound();
            }

            return kurum;
        }

        // POST: api/Kurumlar
        [HttpPost]
        public async Task<ActionResult<Kurum>> PostKurum(Kurum kurum)
        {
            _context.Kurumlar.Add(kurum);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetKurum), new { id = kurum.Id }, kurum);
        }

        // PUT: api/Kurumlar/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKurum(int id, Kurum kurum)
        {
            if (id != kurum.Id)
            {
                return BadRequest();
            }
            _context.Entry(kurum).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Kurumlar.Any(e => e.Id == id))
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

        // DELETE: api/Kurumlar/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKurum(int id)
        {
            var kurum = await _context.Kurumlar.FindAsync(id);
            if (kurum == null)
            {
                return NotFound();
            }
            _context.Kurumlar.Remove(kurum);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}