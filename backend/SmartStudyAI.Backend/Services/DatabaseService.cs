using Microsoft.EntityFrameworkCore;
using SmartStudyAI.Backend.Data;
using SmartStudyAI.Backend.Models;

namespace SmartStudyAI.Backend.Services
{
    /// <summary>
    /// Service class that handles all database interactions for the Smart Study AI application.
    /// This class provides methods to query and manipulate Users and Courses data.
    /// </summary>
    public class DatabaseService
    {
        private readonly ApplicationDbContext _context;

        public DatabaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region User Operations

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>List of all users</returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The user's ID</param>
        /// <returns>The user if found, otherwise null</returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="username">The user's username</param>
        /// <returns>The user if found, otherwise null</returns>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">The user to add</param>
        /// <returns>The added user with ID</returns>
        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user with updated information</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                return false;

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            if (!string.IsNullOrEmpty(user.PasswordHash))
                existingUser.PasswordHash = user.PasswordHash;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="id">The user's ID to delete</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Course Operations

        /// <summary>
        /// Retrieves all courses from the database, including associated user information.
        /// </summary>
        /// <returns>List of all courses with user data</returns>
        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.User)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a course by its ID.
        /// </summary>
        /// <param name="id">The course's ID</param>
        /// <returns>The course if found, otherwise null</returns>
        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Retrieves all courses for a specific user.
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <returns>List of courses for the user</returns>
        public async Task<List<Course>> GetCoursesByUserIdAsync(int userId)
        {
            return await _context.Courses
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new course to the database.
        /// </summary>
        /// <param name="course">The course to add</param>
        /// <returns>The added course with ID</returns>
        public async Task<Course> AddCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        /// <summary>
        /// Updates an existing course in the database.
        /// </summary>
        /// <param name="course">The course with updated information</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> UpdateCourseAsync(Course course)
        {
            var existingCourse = await _context.Courses.FindAsync(course.Id);
            if (existingCourse == null)
                return false;

            existingCourse.Title = course.Title;
            existingCourse.Description = course.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a course from the database.
        /// </summary>
        /// <param name="id">The course's ID to delete</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Checks if a username already exists in the database.
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if exists, otherwise false</returns>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }

        /// <summary>
        /// Checks if an email already exists in the database.
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns>True if exists, otherwise false</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Gets the total count of users in the database.
        /// </summary>
        /// <returns>Number of users</returns>
        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        /// <summary>
        /// Gets the total count of courses in the database.
        /// </summary>
        /// <returns>Number of courses</returns>
        public async Task<int> GetCourseCountAsync()
        {
            return await _context.Courses.CountAsync();
        }

        #endregion
    }
}