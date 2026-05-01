using Microsoft.AspNetCore.Mvc;
using SmartStudyAI.Backend.Data;
using SmartStudyAI.Backend.Models;

namespace SmartStudyAI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FileUploadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file selected." });
            }

            UploadFileRecord uploadedFile = new UploadFileRecord
            {
                FileName = file.FileName,
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadedAt = DateTime.Now
            };

            _context.UploadFileRecords.Add(uploadedFile);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"{file.FileName} has been uploaded",
                fileId = uploadedFile.Id,
                fileName = uploadedFile.FileName
            });
        }

        [HttpGet]
        public IActionResult GetUploadedFiles()
        {
            var files = _context.UploadFileRecords
                .OrderByDescending(file => file.UploadedAt)
                .ToList();

            return Ok(files);
        }
    }
}