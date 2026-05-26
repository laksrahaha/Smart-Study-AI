using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartStudyAI.Backend.Data;
using SmartStudyAI.Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace SmartStudyAI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

    public NotesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get all notes for a user
    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<IEnumerable<Note>>> GetUserNotes(int userId)
    {
        return await _context.Notes
            .Where(n => n.UserId == userId)
            .ToListAsync();
    }

    // View shared note (specific - comes before generic {id})
    [HttpGet("shared/{shareToken}")]
    [AllowAnonymous]
    public async Task<ActionResult> ViewSharedNote(string shareToken)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.ShareToken == shareToken);
        
        if (note == null) return NotFound();
        
        return Ok(new { note.Title, note.Content, note.CourseName });
    }

    // Get single note (generic - comes last)
    [HttpGet("{id}")]
    public async Task<ActionResult<Note>> GetNote(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return NotFound();
        return note;
    }

    // Create note
    [HttpPost]
    public async Task<ActionResult<Note>> CreateNote(Note note)
    {
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
    }

    // Update note
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, Note note)
    {
        if (id != note.Id) return BadRequest();
        _context.Entry(note).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Delete note
    [HttpDelete("note/{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return NotFound();
        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Generate share link
    [HttpPost("{id}/share")]
    public async Task<ActionResult> GenerateShareLink(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return NotFound();
        
        if (string.IsNullOrEmpty(note.ShareToken))
        {
            note.ShareToken = Guid.NewGuid().ToString();
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }
        
        return Ok(new { shareToken = note.ShareToken });
    }
}
}
