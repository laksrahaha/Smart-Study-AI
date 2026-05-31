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
        private readonly PDFExtractionService _pdfExtractionService;

        public AIController(GeminiService geminiService, PDFExtractionService pdfExtractionService)
        {
            _geminiService = geminiService;
            _pdfExtractionService = pdfExtractionService;
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

        [HttpPost("assignment-checklist")]
        public async Task<IActionResult> GenerateAssignmentChecklist([FromBody] AssignmentChecklistRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.AssignmentContent))
            {
                return BadRequest(new
                {
                    error = "Assignment content is required."
                });
            }

            try
            {
                var checklist = await _geminiService.GenerateAssignmentChecklist(request.AssignmentContent);
                return Ok(checklist);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to generate checklist.",
                    details = ex.Message
                });
            }
        }

        [HttpPost("upload-document-checklist")]
        public async Task<IActionResult> UploadDocumentAndGenerateChecklist(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    error = "No file provided."
                });
            }

            try
            {
                // Extract text from PDF/Document
                string extractedText = await _pdfExtractionService.ExtractTextFromDocument(file);

                if (string.IsNullOrWhiteSpace(extractedText))
                {
                    return BadRequest(new
                    {
                        error = "No text could be extracted from the document."
                    });
                }

                // Generate checklist from extracted text
                var checklist = await _geminiService.GenerateAssignmentChecklist(extractedText);

                var response = new DocumentChecklistResponse
                {
                    FileName = file.FileName,
                    UploadedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Checklist = checklist
                };

                return Ok(response);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new
                {
                    error = argEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to process document.",
                    details = ex.Message
                });
            }
        }
    }
}