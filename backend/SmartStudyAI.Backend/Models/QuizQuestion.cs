namespace SmartStudyAI.Backend.Models
{
    public class QuizQuestion
    {
        public string Question {get; set; } = string.Empty;

        public List<string> Options {get; set; } = new();

        public string CorrectAnswer {get; set; } = string.Empty;
    }
}