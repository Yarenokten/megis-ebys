using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IslemGecmisiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IslemGecmisiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/IslemGecmisi/evrak/5
        [HttpGet("evrak/{evrakId}")]
        public async Task<ActionResult<IEnumerable<IslemGecmisi>>> GetGecmisByEvrakId(int evrakId)
        {
            var islemler = await _context.Islemler
                .Where(i => i.EvrakId == evrakId)
                .Include(i => i.Kullanici)
                .OrderBy(i => i.IslemTarihi)
                .ToListAsync();

            if (islemler == null)
            {
                return NotFound();
            }

            return islemler;
        }
    }
}