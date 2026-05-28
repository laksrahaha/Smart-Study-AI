using Microsoft.AspNetCore.Mvc;
using SmartStudyAI.Backend.Services;

namespace SmartStudyAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public CourseController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _databaseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _databaseService.GetAllUsersAsync();
            return Ok(users);
        }
    }
}