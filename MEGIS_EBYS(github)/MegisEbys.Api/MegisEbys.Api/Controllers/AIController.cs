using MegisEbys.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MegisEbys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AIController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("GenerateText")]
        public async Task<IActionResult> GenerateText([FromBody] AiPromptDto promptDto)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(promptDto.Prompt))
            {
                return BadRequest("Lütfen geçerli bir metin giriniz.");
            }

            try
            {
                var apiKey = _configuration["GeminiSettings:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return StatusCode(500, "Gemini API anahtarı yapılandırılmamış.");
                }

                var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-05-20:generateContent?key={apiKey}";

                var placeholdersText = string.Join(", ", promptDto.Placeholders ?? new List<string>());
                var fullPrompt = $"Aşağıdaki anahtar kelimeleri ve şablon yer tutucularını kullanarak resmi bir belge metni oluştur. Cevapta sadece belge metni olsun, açıklama, başlık veya selamlama ifadeleri olmasın. Anahtar kelimeler: '{promptDto.Prompt}'. Yer tutucular: '{placeholdersText}'.";

                var payload = new
                {
                    contents = new[]
                    {
                        new {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    }
                };

                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Yapay zeka servisinden hata alındı: {errorContent}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedJson = JObject.Parse(jsonResponse);
                var generatedText = parsedJson["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                if (string.IsNullOrEmpty(generatedText))
                {
                    return StatusCode(500, "Yapay zeka geçerli bir metin oluşturamadı.");
                }

                var filledPlaceholders = new Dictionary<string, string>();
                if (promptDto.Placeholders != null)
                {
                    // AI'dan gelen metni satırlara ayırarak yer tutuculara daha mantıklı bir şekilde ata
                    var lines = generatedText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();

                    // İlk satırı "Konu" olarak kabul et
                    if (lines.Count > 0 && promptDto.Placeholders.Contains("Konu"))
                    {
                        filledPlaceholders.Add("[Konu]", lines[0]);
                        lines.RemoveAt(0);
                    }

                    // Kalan metni "Belgeİçeriği" olarak kabul et
                    if (lines.Count > 0 && promptDto.Placeholders.Contains("Belgeİçeriği"))
                    {
                        filledPlaceholders.Add("[Belgeİçeriği]", string.Join("\n", lines));
                    }
                    else if (lines.Count > 0 && !filledPlaceholders.ContainsKey("[Konu]"))
                    {
                        // Eğer sadece metin alanı varsa, tüm metni oraya at
                        filledPlaceholders.Add("[Belgeİçeriği]", string.Join("\n", lines));
                    }
                }

                return Ok(new { generatedText = generatedText, filledPlaceholders = filledPlaceholders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"İç sunucu hatası: {ex.Message}");
            }
        }
    }
}
