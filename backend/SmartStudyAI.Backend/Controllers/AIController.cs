using Microsoft.AspNetCore.Mvc;
using SmartStudyAI.Backend.Models;
using SmartStudyAI.Backend.Services;
using System.Text.RegularExpressions;
namespace SmartStudyAI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly GeminiService _geminiService;

        public AIController(GeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] AIRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new
                {
                    error = "Content is required."
                });
            }

            var summary = await _geminiService.GenerateSummary(request.Content);

            return Ok(new AIResponse
            {
                Result = summary
            });
        }

        [HttpPost("flashcards")]
        public async Task<IActionResult> GenerateFlashcards([FromBody] AIRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Content is required." });

            int count = 8;
            string noteContent = request.Content;

            var match = Regex.Match(
                request.Content,
                @"Generate (\d+) flash cards\.\s*Content:\s*(.*)",
                RegexOptions.Singleline | RegexOptions.IgnoreCase
            );

            if (match.Success)
            { count = int.TryParse(match.Groups[1].Value, out var parsed) ? parsed : 8;
                noteContent = match.Groups[2].Value.Trim();
            }

            if (string.IsNullOrWhiteSpace(noteContent))
                return BadRequest(new { error = "Note content is empty." });

            try
            {
                var cards = await _geminiService.GenerateFlashcards(noteContent, count);
                return Ok(cards);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}