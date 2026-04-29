using Microsoft.AspNetCore.Mvc;
using SmartStudyAI.Models;
using System.Collections.Generic;

namespace SmartStudyAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private static List<Course> courses = new List<Course>();

        // GET all courses
        [HttpGet]
        public IActionResult GetCourses()
        {
            return Ok(courses);
        }

        // POST create course
        [HttpPost]
        public IActionResult CreateCourse([FromBody] Course course)
        {
            course.Id = courses.Count + 1;
            courses.Add(course);

            return Ok(course);
        }
    }
}