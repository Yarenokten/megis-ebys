using MegisEbys.Api.Data;
using MegisEbys.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using MegisEbys.Api.Dtos;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SablonlarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SablonlarController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Sablonlar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sablon>>> GetSablonlar()
        {
            return await _context.Sablonlar.OrderBy(s => s.Ad).ToListAsync();
        }

        // POST: api/Sablonlar/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.FirstOrDefault();
                var ad = formCollection["ad"].FirstOrDefault();

                if (file == null || file.Length == 0)
                    return BadRequest("Yüklenecek dosya bulunamadı.");
                if (string.IsNullOrEmpty(ad))
                    return BadRequest("Şablon adı boş olamaz.");

                if (string.IsNullOrEmpty(_env.WebRootPath))
                {
                    _env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    if (!Directory.Exists(_env.WebRootPath)) Directory.CreateDirectory(_env.WebRootPath);
                }

                var uploadsPath = Path.Combine(_env.WebRootPath, "sablonlar");
                if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var sablon = new Sablon
                {
                    Ad = ad,
                    DosyaYolu = Path.Combine("/sablonlar", uniqueFileName).Replace('\\', '/')
                };

                _context.Sablonlar.Add(sablon);
                await _context.SaveChangesAsync();

                return Ok(sablon);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İç sunucu hatası: {ex.Message}");
            }
        }

        // GET: api/Sablonlar/download/5
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var sablon = await _context.Sablonlar.FindAsync(id);
            if (sablon == null || string.IsNullOrEmpty(sablon.DosyaYolu))
                return NotFound("Şablon dosyası bulunamadı.");

            var filePath = Path.Combine(_env.WebRootPath, sablon.DosyaYolu.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound("Dosya sunucuda bulunamadı.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            // DÜZELTME: Dosya adını güvenli bir şekilde al
            var originalFileName = Path.GetFileName(filePath);
            var index = originalFileName.IndexOf('_');
            var friendlyFileName = index != -1 ? originalFileName.Substring(index + 1) : originalFileName;

            return File(memory, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", friendlyFileName);
        }

        // DELETE: api/Sablonlar/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSablon(int id)
        {
            var sablon = await _context.Sablonlar.FindAsync(id);
            if (sablon == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, sablon.DosyaYolu.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Sablonlar.Remove(sablon);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Sablonlar/fill/5
        [HttpPost("fill/{id}")]
        public async Task<IActionResult> FillTemplate(int id, [FromBody] FillSablonDto dto)
        {
            var sablon = await _context.Sablonlar.FindAsync(id);
            if (sablon == null) return NotFound("Şablon bulunamadı.");

            var templatePath = Path.Combine(_env.WebRootPath, sablon.DosyaYolu.TrimStart('/'));
            if (!System.IO.File.Exists(templatePath)) return NotFound("Şablon dosyası sunucuda bulunamadı.");

            var newFileName = Guid.NewGuid().ToString() + "_doldurulmus.docx";
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

            var newFilePath = Path.Combine(uploadsPath, newFileName);

            System.IO.File.Copy(templatePath, newFilePath, true);

            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(newFilePath, true))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    foreach (var text in body.Descendants<Text>())
                    {
                        foreach (var placeholder in dto.Placeholders)
                        {
                            if (text.Text.Contains(placeholder.Key))
                            {
                                text.Text = text.Text.Replace(placeholder.Key, placeholder.Value);
                            }
                        }
                    }
                    wordDoc.MainDocumentPart.Document.Save();
                }

                var resultPath = Path.Combine("/uploads", newFileName).Replace('\\', '/');
                return Ok(new { filePath = resultPath });
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(newFilePath)) System.IO.File.Delete(newFilePath);
                return StatusCode(500, $"Şablon doldurulurken hata oluştu: {ex.Message}");
            }
        }

        // GET: api/Sablonlar/get-text/5
        [HttpGet("get-text/{id}")]
        public async Task<IActionResult> GetTemplateText(int id)
        {
            var sablon = await _context.Sablonlar.FindAsync(id);
            if (sablon == null || string.IsNullOrEmpty(sablon.DosyaYolu))
                return NotFound("Şablon dosyası bulunamadı.");

            var filePath = Path.Combine(_env.WebRootPath, sablon.DosyaYolu.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound("Dosya sunucuda bulunamadı.");

            try
            {
                string documentText;
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    documentText = wordDoc.MainDocumentPart.Document.Body.InnerText;
                }

                return Ok(new { text = documentText });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Şablon metni okunurken bir hata oluştu: {ex.Message}");
            }
        }

        // YENİ METOT: Şablonun yer tutucularını döndürür
        [HttpGet("placeholders/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> GetTemplatePlaceholders(int id)
        {
            var sablon = await _context.Sablonlar.FindAsync(id);
            if (sablon == null || string.IsNullOrEmpty(sablon.DosyaYolu))
                return NotFound("Şablon dosyası bulunamadı.");

            var filePath = Path.Combine(_env.WebRootPath, sablon.DosyaYolu.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound("Dosya sunucuda bulunamadı.");

            try
            {
                List<string> placeholders = new List<string>();
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    var text = body.InnerText;
                    // Regex ile "[...]" formatındaki yer tutucuları bul
                    var matches = Regex.Matches(text, @"\[(.*?)\]");
                    foreach (Match match in matches)
                    {
                        placeholders.Add(match.Groups[1].Value);
                    }
                }

                return Ok(placeholders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Şablon yer tutucuları okunurken bir hata oluştu: {ex.Message}");
            }
        }
    }
}
