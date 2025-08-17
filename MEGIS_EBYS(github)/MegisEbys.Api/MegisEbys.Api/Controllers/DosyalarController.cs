using MegisEbys.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using QuestPDF.Fluent;
using MegisEbys.Api.Documents;
using System.Linq;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DosyalarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DosyalarController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("upload")]
        // DÜZELTME: Dosyayı parametreden almak yerine, doğrudan Request.Form.Files koleksiyonundan okuyoruz.
        // Bu, tüm model bağlama ve Swagger hatalarını çözen en sağlam yöntemdir.
        public async Task<IActionResult> Upload()
        {
            if (Request.Form.Files.Count == 0)
            {
                return BadRequest("Yüklenecek dosya bulunamadı.");
            }

            var file = Request.Form.Files[0];

            if (file.Length == 0)
            {
                return BadRequest("Boş dosya yüklenemez.");
            }

            if (string.IsNullOrEmpty(_env.WebRootPath))
            {
                _env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (!Directory.Exists(_env.WebRootPath))
                {
                    Directory.CreateDirectory(_env.WebRootPath);
                }
            }

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Dosya kaydedilirken bir hata oluştu: {ex.Message}");
            }

            return Ok(new { filePath = Path.Combine("/uploads", uniqueFileName).Replace('\\', '/') });
        }

        [HttpGet("download/{evrakId}")]
        public async Task<IActionResult> Download(int evrakId)
        {
            var evrak = await _context.Evraklar.FindAsync(evrakId);
            if (evrak == null || string.IsNullOrEmpty(evrak.DosyaYolu))
            {
                return NotFound("Evraka ait dosya bulunamadı.");
            }

            var filePath = Path.Combine(_env.WebRootPath, evrak.DosyaYolu.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Dosya sunucuda bulunamadı.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var originalFileName = Path.GetFileName(filePath);
            var index = originalFileName.IndexOf('_');
            var friendlyFileName = index != -1 ? originalFileName.Substring(index + 1) : originalFileName;

            return File(memory, "application/octet-stream", friendlyFileName);
        }

        [HttpGet("generate-pdf/{evrakId}")]
        public async Task<IActionResult> GenerateAndDownloadPdf(int evrakId)
        {
            var evrak = await _context.Evraklar
                .Include(e => e.GonderenKullanici)
                .Include(e => e.AliciKullanici)
                .Include(e => e.AliciBirim)
                .Include(e => e.EvrakTuru)
                .FirstOrDefaultAsync(e => e.Id == evrakId);

            if (evrak == null)
            {
                return NotFound("Evrak bulunamadı.");
            }

            var document = new EvrakPdfDocument(evrak);
            var pdfBytes = document.GeneratePdf();
            var fileName = $"{evrak.Konu.Substring(0, Math.Min(evrak.Konu.Length, 20))}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
    }
}
