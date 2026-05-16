using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SmartStudyAI.Backend.Services
{
    public class GeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateSummary(string content)
        {
            string apiKey = _configuration["Gemini:ApiKey"] ?? "";

            string url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = $"Summarize these study notes clearly for a student:\n\n{content}"
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);

            var requestContent = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(url, requestContent);

          

            if (!response.IsSuccessStatusCode)
            {
                return $"Gemini API Error: {response.StatusCode}";
            }

            var responseString = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseString);

            var result =
                doc.RootElement
                   .GetProperty("candidates")[0]
                   .GetProperty("content")
                   .GetProperty("parts")[0]
                   .GetProperty("text")
                   .GetString();

            return result ?? "No summary generated.";
        }
    }
}