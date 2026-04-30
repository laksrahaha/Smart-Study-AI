using Microsoft.EntityFrameworkCore;
using SmartStudyAI.Backend.Data;
using SmartStudyAI.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Auto-migrate database on startup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add SQL Server DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    
    // Seed sample data if database is empty
    if (!db.Users.Any())
    {
        // Run seed script
        var seedSql = File.ReadAllText("seed-data.sql");
        db.Database.ExecuteSqlRaw(seedSql);
        Console.WriteLine("Sample data seeded from seed-data.sql!");
    }
}

app.UseCors();

// Existing endpoint
app.MapGet("/", () => "Hello World!");

// NEW: API endpoints for Courses
app.MapGet("/api/courses", async (ApplicationDbContext db) =>
    await db.Courses.Include(c => c.User).ToListAsync());

app.MapPost("/api/courses", async (ApplicationDbContext db, Course course) =>
{
    db.Courses.Add(course);
    await db.SaveChangesAsync();
    return Results.Created($"/api/courses/{course.Id}", course);
});

// NEW: API endpoints for Users (optional)
app.MapGet("/api/users", async (ApplicationDbContext db) =>
    await db.Users.ToListAsync());

app.MapPost("/api/users", async (ApplicationDbContext db, User user) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", user);
});

app.Run();