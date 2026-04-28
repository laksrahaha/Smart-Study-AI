namespace SmartStudyAI.Backend.modelArchitecture;

public class userResponse
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string StudyLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}