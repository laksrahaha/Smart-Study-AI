using SmartStudyAI.Backend.modelArchitecture;

var builder = WebApplication.CreateBuilder(args);

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



app.MapPost("/api/account/register", (registerRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.FirstName) ||
        string.IsNullOrWhiteSpace(request.LastName) ||
        string.IsNullOrWhiteSpace(request.Email) ||
        string.IsNullOrWhiteSpace(request.StudyLevel) ||
        string.IsNullOrWhiteSpace(request.Password) ||
        string.IsNullOrWhiteSpace(request.ConfirmPassword))
    {
        return Results.BadRequest(new { message = "All registration fields are required." });
    }

    if (!request.Email.Contains("@") || !request.Email.Contains("."))
    {
        return Results.BadRequest(new { message = "Please enter a valid email address." });
    }

    if (request.Password.Length < 6)
    {
        return Results.BadRequest(new { message = "Password must be at least 6 characters long." });
    }

    if (request.Password != request.ConfirmPassword)
    {
        return Results.BadRequest(new { message = "Passwords do not match." });
    }

    bool emailExists = registeredUsers.Any(user =>
        user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

    if (emailExists)
    {
        return Results.BadRequest(new { message = "An account with this email already exists." });
    }

    RegisteredUser newUser = new RegisteredUser
    {
        UserId = registeredUsers.Count + 1,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email,
        StudyLevel = request.StudyLevel,
        Password = request.Password
    };

    registeredUsers.Add(newUser);

    userResponse response = new userResponse
    {
        UserId = newUser.UserId,
        FirstName = newUser.FirstName,
        LastName = newUser.LastName,
        Email = newUser.Email,
        StudyLevel = newUser.StudyLevel,
        Message = "Account registered successfully."
    };

    return Results.Ok(response);
});


app.Run();

public class RegisteredUser
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StudyLevel{get; set;} = string.Empty;
    public string Password { get; set; } = string.Empty;
}