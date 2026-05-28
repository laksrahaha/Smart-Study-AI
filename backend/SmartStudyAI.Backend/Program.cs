using Microsoft.EntityFrameworkCore;
using SmartStudyAI.Backend.Data;
using SmartStudyAI.Backend.Models;
using SmartStudyAI.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=smartstudy.db"));

builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<GeminiService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-migrate and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS UploadFileRecords (
            Id INTEGER NOT NULL CONSTRAINT PK_UploadFileRecords PRIMARY KEY AUTOINCREMENT,
            FileName TEXT NOT NULL,
            FileSize INTEGER NOT NULL,
            ContentType TEXT NOT NULL,
            UploadedAt TEXT NOT NULL
        );
    ");

    if (!db.Users.Any())
    {
        var seedSql = File.ReadAllText("seed-data.sql");
        db.Database.ExecuteSqlRaw(seedSql);
        Console.WriteLine("Sample data seeded from seed-data.sql!");
    }
}

app.UseCors();

// ── Controllers handle: Notes, Community, FileUpload ──────────────
app.MapControllers();

// ── Minimal API: root ─────────────────────────────────────────────
app.MapGet("/", () => "Hello World!");

// ── Minimal API: Courses ──────────────────────────────────────────
app.MapGet("/api/courses", async (ApplicationDbContext db) =>
    await db.Courses.Include(c => c.User).ToListAsync());

app.MapPost("/api/courses", async (ApplicationDbContext db, Course course) =>
{
    db.Courses.Add(course);
    await db.SaveChangesAsync();
    return Results.Created($"/api/courses/{course.Id}", course);
});

// ── Minimal API: Users ────────────────────────────────────────────
app.MapGet("/api/users", async (ApplicationDbContext db) =>
    await db.Users.ToListAsync());

app.MapGet("/api/users/{id}", async (ApplicationDbContext db, int id) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound(new { error = "User not found." });
    return Results.Ok(user);
});

app.MapPost("/api/users", async (ApplicationDbContext db, User user) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", user);
});

app.MapPut("/api/users/{id}", async (ApplicationDbContext db, int id, User updatedUser) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound(new { error = "User not found." });

    user.Username = updatedUser.Username;
    user.Email    = updatedUser.Email;
    if (!string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
        user.PasswordHash = updatedUser.PasswordHash;

    await db.SaveChangesAsync();
    return Results.Ok(user);
});

// ── Minimal API: Notes ────────────────────────────────────────────
app.MapGet("/api/notes", async (ApplicationDbContext db) =>
    await db.Notes.ToListAsync());

app.MapGet("/api/notes/user/{userId}", async (ApplicationDbContext db, int userId) =>
    await db.Notes
        .Where(n => n.UserId == userId)
        .OrderByDescending(n => n.UpdatedAt)
        .ToListAsync());

app.MapGet("/api/notes/shared/{shareToken}", async (ApplicationDbContext db, string shareToken) =>
{
    var note = await db.Notes.FirstOrDefaultAsync(n => n.ShareToken == shareToken);
    if (note == null) return Results.NotFound();
    return Results.Ok(new { note.Title, note.Content, note.CourseName });
});

app.MapGet("/api/notes/{id:int}", async (ApplicationDbContext db, int id) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null) return Results.NotFound();
    return Results.Ok(note);
});

app.MapPost("/api/notes", async (ApplicationDbContext db, Note note) =>
{
    note.UpdatedAt = DateTime.UtcNow;
    db.Notes.Add(note);
    await db.SaveChangesAsync();
    return Results.Created($"/api/notes/{note.Id}", note);
});

app.MapPut("/api/notes/{id:int}", async (ApplicationDbContext db, int id, Note updatedNote) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null) return Results.NotFound();

    note.Title      = updatedNote.Title;
    note.Content    = updatedNote.Content;
    note.CourseName = updatedNote.CourseName;
    note.CourseId   = updatedNote.CourseId;
    note.UpdatedAt  = DateTime.UtcNow;

    await db.SaveChangesAsync();
    return Results.Ok(note);
});

app.MapDelete("/api/notes/{id:int}", async (ApplicationDbContext db, int id) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null) return Results.NotFound();
    db.Notes.Remove(note);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/api/notes/{id:int}/share", async (ApplicationDbContext db, int id) =>
{
    var note = await db.Notes.FindAsync(id);
    if (note == null) return Results.NotFound();

    if (string.IsNullOrEmpty(note.ShareToken))
    {
        note.ShareToken = Guid.NewGuid().ToString();
        await db.SaveChangesAsync();
    }

    return Results.Ok(new { shareToken = note.ShareToken });
});

// ── Minimal API: File Uploads ─────────────────────────────────────
app.MapPost("/api/fileupload/upload", async (ApplicationDbContext db, IFormFile file) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "No file selected." });

    var uploadedFile = new UploadFileRecord
    {
        FileName    = file.FileName,
        FileSize    = file.Length,
        ContentType = file.ContentType ?? "",
        UploadedAt  = DateTime.UtcNow
    };

    db.UploadFileRecords.Add(uploadedFile);
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        message  = $"{file.FileName} has been uploaded",
        fileId   = uploadedFile.Id,
        fileName = uploadedFile.FileName
    });
}).DisableAntiforgery();

app.MapGet("/api/fileupload", async (ApplicationDbContext db) =>
    await db.UploadFileRecords
        .OrderByDescending(f => f.UploadedAt)
        .ToListAsync());

app.Run();
