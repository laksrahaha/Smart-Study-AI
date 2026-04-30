using Microsoft.AspNetCore.Mvc;
using SmartStudyAI.Models;
using System.Collections.Generic;
using System.Linq;

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
            course.LastAccessed = DateTime.Now;

            courses.Add(course);
            return Ok(course);
        }

        // PUT rename course
        [HttpPut("{id}")]
        public IActionResult UpdateCourse(int id, [FromBody] Course updated)
        {
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
                return NotFound();

            course.Title = updated.Title;
            return Ok(course);
        }

        // DELETE course
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
                return NotFound();

            courses.Remove(course);
            return Ok();
        }

        // SEARCH + SORT
        [HttpGet("search")]
        public IActionResult SearchCourses(string query, string sort)
        {
            var result = courses.AsQueryable();

            if (!string.IsNullOrEmpty(query))
                result = result.Where(c => c.Title.ToLower().Contains(query.ToLower()));

            if (sort == "asc")
                result = result.OrderBy(c => c.Title);
            else if (sort == "desc")
                result = result.OrderByDescending(c => c.Title);

            return Ok(result.ToList());
        }

        // SWITCH COURSE (track last accessed)
        [HttpPost("switch/{id}")]
        public IActionResult SwitchCourse(int id)
        {
            var course = courses.FirstOrDefault(c => c.Id == id);

            if (course == null)
                return NotFound();

            course.LastAccessed = DateTime.Now;
            return Ok(course);
        }

        // GET last opened course
        [HttpGet("last")]
        public IActionResult GetLastCourse()
        {
            var last = courses.OrderByDescending(c => c.LastAccessed).FirstOrDefault();
            return Ok(last);
        }
    }
}