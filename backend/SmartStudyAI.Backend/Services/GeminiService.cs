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
            // Get Gemini API key from appsettings.json
            string apiKey = _configuration["Gemini:ApiKey"] ?? "";

            // Gemini API endpoint
            string url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            // AI prompt sent to Gemini
            string prompt =
                $"Summarize these study notes clearly and simply for a student:\n\n{content}";

            // Request body sent to Gemini API
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                },

                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 300
                }
            };

            // Convert C# object into JSON
            var json = JsonSerializer.Serialize(requestBody);

            var requestContent = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            // Send request to Gemini API
            var response = await _httpClient.PostAsync(url, requestContent);

            // Handle API errors
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                return $"Gemini API Error: {response.StatusCode}\n{error}";
            }

            // Read Gemini response
            var responseString = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseString);

            // Extract generated summary text
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