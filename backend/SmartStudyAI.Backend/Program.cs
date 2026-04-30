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