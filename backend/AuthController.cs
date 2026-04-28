using Microsoft.AspNetCore.Mvc;

namespace SmartStudyAI.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // need database
        private static readonly List<User> _users = new()
        {
            new User { Email = "test@student.com", Password = "COMP602", Username = "TestStudent" }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email == request.Email &&
                u.Password == request.Password);

            if (user == null)
                return Unauthorized(new { message = "Incorrect email or password." });

            return Ok(new {
                message = "Login successful",
                username = user.Username,
                email = user.Email
            });
        }
    }

    public class User
    {
        public string Email    { get; set; } = "";
        public string Password { get; set; } = "";
        public string Username { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Email    { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
