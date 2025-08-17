using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisDepartmanlarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DisDepartmanlarController(ApplicationDbContext context) => _context = context;

        // GET: api/DisDepartmanlar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisDepartman>>> GetDisDepartmanlar()
        {
            return await _context.DisDepartmanlar.Include(d => d.Kurum).ToListAsync();
        }

        // GET: api/DisDepartmanlar/kurum/5
        [HttpGet("kurum/{kurumId}")]
        public async Task<ActionResult<IEnumerable<DisDepartman>>> GetDepartmanlarByKurum(int kurumId)
        {
            return await _context.DisDepartmanlar
                .Where(d => d.KurumId == kurumId)
                .OrderBy(d => d.Ad)
                .ToListAsync();
        }

        // POST: api/DisDepartmanlar
        [HttpPost]
        public async Task<ActionResult<DisDepartman>> PostDisDepartman(DisDepartman disDepartman)
        {
            _context.DisDepartmanlar.Add(disDepartman);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDisDepartmanlar), new { id = disDepartman.Id }, disDepartman);
        }

        // PUT: api/DisDepartmanlar/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisDepartman(int id, DisDepartman disDepartman)
        {
            if (id != disDepartman.Id)
            {
                return BadRequest();
            }
            _context.Entry(disDepartman).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DisDepartmanlar.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // DELETE: api/DisDepartmanlar/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDisDepartman(int id)
        {
            var disDepartman = await _context.DisDepartmanlar.FindAsync(id);
            if (disDepartman == null)
            {
                return NotFound();
            }
            _context.DisDepartmanlar.Remove(disDepartman);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}