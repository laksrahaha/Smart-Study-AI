using Microsoft.AspNetCore.Mvc;
using SmartStudyAI.Backend.Models;
using SmartStudyAI.Backend.Services;

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
    }
}