using Microsoft.AspNetCore.Mvc;
using SmartStudyAI.Backend.Data;
using SmartStudyAI.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartStudyAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses.Include(c => c.User).ToListAsync();
            return Ok(courses);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}