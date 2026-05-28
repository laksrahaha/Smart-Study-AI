// TranslationController.cs
// ASP.NET Core Web API – Translation Proxy
// Calls MyMemory API on behalf of the frontend

using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SuperSmartStudyAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TranslateController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET /api/translate?word=photosynthesis&target=zh
        [HttpGet]
        public async Task<IActionResult> Translate(
            [FromQuery] string word,
            [FromQuery] string target = "zh")
        {
            if (string.IsNullOrWhiteSpace(word))
                return BadRequest(new { error = "Word is required." });

            // Validate target language
            var allowedLanguages = new[] { "zh", "es", "fr", "ja", "ko", "ar", "hi", "de" };
            if (!Array.Exists(allowedLanguages, l => l == target))
                return BadRequest(new { error = "Unsupported target language." });

            try
            {
                var client = _httpClientFactory.CreateClient();

                // Build MyMemory API URL
                var url = $"https://api.mymemory.translated.net/get" +
                           $"?q={Uri.EscapeDataString(word)}" +
                           $"&langpair=en|{target}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return StatusCode(502, new { error = "Translation service unavailable." });

                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonDocument.Parse(json);
                var root = parsed.RootElement;

                // Extract translated text from MyMemory response
                var translatedText = root
                    .GetProperty("responseData")
                    .GetProperty("translatedText")
                    .GetString();

                // Check response status from MyMemory
                var responseStatus = root.GetProperty("responseStatus").GetInt32();
                if (responseStatus != 200 || string.IsNullOrEmpty(translatedText))
                    return Ok(new { translation = "Translation unavailable." });

                return Ok(new
                {
                    word        = word,
                    translation = translatedText,
                    targetLang  = target
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new { error = "Could not reach translation service." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
