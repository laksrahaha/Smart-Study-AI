<<<<<<< HEAD
using SmartStudyAI.Backend.modelArchitecture;

var builder = WebApplication.CreateBuilder(args);
feature/login-page(built-by-Louise-Wang)

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
=======

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

List<RegisteredUser> registeredUsers = new();

app.MapGet("/", () =>
{
    return "Smart Study AI Backend is running.";
});

app.MapPost("/api/account/register", (registerRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.FullName) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.Password) ||
        string.IsNullOrWhiteSpace(request.ConfirmPassword))
    {
        return Results.BadRequest("All registration fields are required.");
    }

    if (request.Password != request.ConfirmPassword)
    {
        return Results.BadRequest("Passwords do not match.");
    }

    RegisteredUser newUser = new RegisteredUser
    {
        UserId = registeredUsers.Count + 1,
        FullName = request.FullName,
        Email = request.Email,
        Password = request.Password
    };

    registeredUsers.Add(newUser);

    userResponse response = new userResponse
    {
        UserId = newUser.UserId,
        FullName = newUser.FullName,
        Email = newUser.Email
    };

    return Results.Ok(response);
});

app.MapGet("/api/account/users", () =>
{
    var safeUsers = registeredUsers.Select(user => new userResponse
    {
        UserId = user.UserId,
        FullName = user.FullName,
        Email = user.Email
    });

    return Results.Ok(safeUsers);

  main
});

var app = builder.Build();
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();

public class RegisteredUser
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
=======
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
>>>>>>> feature/Database-Phi
