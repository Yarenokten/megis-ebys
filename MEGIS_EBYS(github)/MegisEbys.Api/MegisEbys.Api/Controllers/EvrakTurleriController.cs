using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvrakTurleriController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EvrakTurleriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EvrakTurleri
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvrakTuru>>> GetEvrakTurleri()
        {
            return await _context.EvrakTurleri.ToListAsync();
        }

        // GET: api/EvrakTurleri/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EvrakTuru>> GetEvrakTuru(int id)
        {
            var evrakTuru = await _context.EvrakTurleri.FindAsync(id);

            if (evrakTuru == null)
            {
                return NotFound();
            }

            return evrakTuru;
        }

        // POST: api/EvrakTurleri
        [HttpPost]
        public async Task<ActionResult<EvrakTuru>> PostEvrakTuru(EvrakTuru evrakTuru)
        {
            _context.EvrakTurleri.Add(evrakTuru);
            await _context.SaveChangesAsync();

            // CreatedAtAction'da GetEvrakTuru metoduna referans veriyoruz.
            return CreatedAtAction(nameof(GetEvrakTuru), new { id = evrakTuru.Id }, evrakTuru);
        }

        // PUT: api/EvrakTurleri/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvrakTuru(int id, EvrakTuru evrakTuru)
        {
            if (id != evrakTuru.Id)
            {
                return BadRequest();
            }

            _context.Entry(evrakTuru).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EvrakTurleri.Any(e => e.Id == id))
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

        // DELETE: api/EvrakTurleri/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvrakTuru(int id)
        {
            var evrakTuru = await _context.EvrakTurleri.FindAsync(id);
            if (evrakTuru == null)
            {
                return NotFound();
            }

            _context.EvrakTurleri.Remove(evrakTuru);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
