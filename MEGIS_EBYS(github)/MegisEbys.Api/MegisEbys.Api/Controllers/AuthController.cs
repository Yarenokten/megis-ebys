using MegisEbys.Api.Data;
using MegisEbys.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // ... (mevcut kodun başı aynı) ...

            var user = await _context.Kullanicilar
                                     .FirstOrDefaultAsync(u => u.Eposta == loginDto.Email);

            if (user == null || user.Sifre != loginDto.Password)
            {
                return Unauthorized(new { message = "Geçersiz e-posta veya şifre." });
            }

            if (!user.AktifMi)
            {
                return Unauthorized(new { message = "Kullanıcı hesabı pasif durumdadır." });
            }

            // Başarılı girişte kullanıcı bilgilerini (şifre hariç) geri dön
            return Ok(new
            {
                id = user.Id,
                adSoyad = user.AdSoyad,
                eposta = user.Eposta,
                yetki = user.Yetki,
                birimId = user.BirimId // DÜZELTME: birimId eklendi
            });
        }

    }
}