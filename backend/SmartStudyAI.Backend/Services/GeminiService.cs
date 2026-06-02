using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using SmartStudyAI.Backend.Models;

namespace SmartStudyAI.Backend.Services
{
    public class GeminiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private const string GeminiUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

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
                $"{GeminiUrl}?key={apiKey}";

            // AI prompt sent to Gemini
             string prompt =
    $"Summarize the following student study notes.\n" +
    $"Return ONLY the summary — no preamble, no meta-commentary.\n" +
    $"Format: 3–5 concise bullet points covering the key concepts.\n" +
    $"Each bullet should be one sentence. Use plain language.\n\n" +
    $"Notes:\n{content}";

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
                    maxOutputTokens = 1024
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

        public async Task<List<FlashCard>> GenerateFlashcards(string content, int count)
{
    string apiKey = _configuration["Gemini:ApiKey"] ?? "";
    string url = $"{GeminiUrl}?key={apiKey}";

    string prompt =
        $"Create exactly {count} flashcards from the following study notes.\n" +
        $"Return ONLY a valid JSON array. Each element must have exactly two string fields: " +
        $"\"front\" (a concise question) and \"back\" (a clear answer).\n" +
        $"Do not wrap the JSON in markdown code blocks. Output the raw JSON array only.\n\n" +
        $"Study notes:\n{content}";

    var requestBody = new
    {
        contents = new[]
        {
            new { role = "user", parts = new[] { new { text = prompt } } }
        },
        generationConfig = new
        {
            temperature = 0.7,
            maxOutputTokens = 2048,
            responseMimeType = "application/json"
        }
    };

    var json = JsonSerializer.Serialize(requestBody);
    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
    var response = await _httpClient.PostAsync(url, httpContent);

    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadAsStringAsync();
        throw new Exception($"Gemini API Error: {response.StatusCode} — {error}");
    }

    var responseString = await response.Content.ReadAsStringAsync();
    using JsonDocument doc = JsonDocument.Parse(responseString);

    var rawText = doc.RootElement
        .GetProperty("candidates")[0]
        .GetProperty("content")
        .GetProperty("parts")[0]
        .GetProperty("text")
        .GetString() ?? "[]";

    rawText = rawText.Trim();
    rawText = Regex.Replace(rawText, @"^```[a-z]*\n?", "", RegexOptions.Multiline).Trim();
    rawText = rawText.TrimEnd('`').Trim();

    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    return JsonSerializer.Deserialize<List<FlashCard>>(rawText, options) ?? new List<FlashCard>();
}

    }
}
