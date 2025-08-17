using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MegisEbys.Api.Data;
using MegisEbys.Api.Models;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirimlerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BirimlerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Birimler
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Birim>>> GetBirimler()
        {
            return await _context.Birimler.ToListAsync();
        }

        // GET: api/Birimler/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Birim>> GetBirim(int id)
        {
            var birim = await _context.Birimler.FindAsync(id);

            if (birim == null)
            {
                return NotFound();
            }

            return birim;
        }

        // PUT: api/Birimler/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBirim(int id, Birim birim)
        {
            if (id != birim.Id)
            {
                return BadRequest();
            }

            _context.Entry(birim).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BirimExists(id))
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

        // POST: api/Birimler
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Birim>> PostBirim(Birim birim)
        {
            _context.Birimler.Add(birim);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBirim", new { id = birim.Id }, birim);
        }

        // DELETE: api/Birimler/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBirim(int id)
        {
            var birim = await _context.Birimler.FindAsync(id);
            if (birim == null)
            {
                return NotFound();
            }

            _context.Birimler.Remove(birim);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BirimExists(int id)
        {
            return _context.Birimler.Any(e => e.Id == id);
        }
    }
}
