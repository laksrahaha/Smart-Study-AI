[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest(new { error = "No file selected." });

    // Return the exact message you requested
    return Ok(new { message = $"{file.FileName} has been uploaded" });
}