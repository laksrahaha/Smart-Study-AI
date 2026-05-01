namespace SmartStudyAI.Backend.Models
{
    public class Note
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string CourseName { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }

        public Course? Course { get; set; }
    }
}