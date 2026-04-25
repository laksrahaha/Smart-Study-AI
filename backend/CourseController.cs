using Microsoft.AspNetCore.Mvc;

namespace SmartStudyAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCourses()
        {
            return Ok("Course workspace working!");
        }
    }
}