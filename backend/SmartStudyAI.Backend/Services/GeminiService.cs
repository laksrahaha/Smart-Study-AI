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
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            
            // Load API key from file
            string keyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gemini-api-key.txt");
            if (File.Exists(keyFilePath))
            {
                _apiKey = File.ReadAllText(keyFilePath).Trim();
            }
            else
            {
                throw new FileNotFoundException($"API key file not found at {keyFilePath}. Please create gemini-api-key.txt with your Gemini API key.");
            }
        }

        public async Task<string> GenerateSummary(string content)
        {
            // Get Gemini API key from file
            string apiKey = _apiKey;

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

        public async Task<AssignmentChecklistResponse> GenerateAssignmentChecklist(string assignmentContent)
        {
            // Get Gemini API key from file
            string apiKey = _apiKey;

            // Gemini API endpoint
            string url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            // AI prompt sent to Gemini to parse assignment and generate checklist
            string prompt = $@"You are an expert teaching assistant. Analyze the following assignment and create a detailed checklist with completion guide.

ASSIGNMENT:
{assignmentContent}

Please provide the response in EXACTLY this JSON format (no markdown, no code blocks, pure JSON):
{{
    ""assignmentTitle"": ""[extracted assignment title or 'Untitled Assignment' if not found]"",
    ""dueDate"": ""[extracted due date or 'Not specified' if not found]"",
    ""estimatedTimeMinutes"": [estimated time to complete in minutes as a number],
    ""overallGuidance"": ""[2-3 sentence overview of what the assignment requires and best approach]"",
    ""checklist"": [
        {{
            ""id"": 1,
            ""task"": ""[specific subtask description]"",
            ""guide"": ""[step-by-step guide or tips to complete this task]"",
            ""priority"": 1
        }}
    ]
}}

Ensure each checklist item has:
- A clear, actionable task
- Detailed step-by-step guidance
- Priority (1=High/Critical, 2=Medium/Important, 3=Low/Optional)

Generate 5-10 checklist items depending on assignment complexity.";

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
                    maxOutputTokens = 2000
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
                throw new Exception($"Gemini API Error: {response.StatusCode}\n{error}");
            }

            // Read Gemini response
            var responseString = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseString);

            // Extract generated checklist JSON from Gemini response
            var resultText =
                doc.RootElement
                   .GetProperty("candidates")[0]
                   .GetProperty("content")
                   .GetProperty("parts")[0]
                   .GetProperty("text")
                   .GetString();

            if (string.IsNullOrEmpty(resultText))
            {
                throw new Exception("No checklist generated from AI.");
            }

            // Parse the JSON response from Gemini
            using JsonDocument checklistDoc = JsonDocument.Parse(resultText);
            var checklistElement = checklistDoc.RootElement;

            var checklistResponse = new AssignmentChecklistResponse
            {
                AssignmentTitle = checklistElement.GetProperty("assignmentTitle").GetString() ?? "Untitled Assignment",
                DueDate = checklistElement.GetProperty("dueDate").GetString() ?? "Not specified",
                EstimatedTimeMinutes = checklistElement.GetProperty("estimatedTimeMinutes").GetInt32(),
                OverallGuidance = checklistElement.GetProperty("overallGuidance").GetString() ?? "",
                Checklist = new List<ChecklistItem>()
            };

            // Parse checklist items
            var checklistItems = checklistElement.GetProperty("checklist");
            foreach (var item in checklistItems.EnumerateArray())
            {
                checklistResponse.Checklist.Add(new ChecklistItem
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Task = item.GetProperty("task").GetString() ?? "",
                    Guide = item.GetProperty("guide").GetString() ?? "",
                    Priority = item.GetProperty("priority").GetInt32(),
                    IsCompleted = false
                });
            }

            return checklistResponse;
        }
        public async Task<List<FlashCard>> GenerateFlashcards(string content, int count)
{
    string apiKey = _configuration["Gemini:ApiKey"] ?? "";
    string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

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
