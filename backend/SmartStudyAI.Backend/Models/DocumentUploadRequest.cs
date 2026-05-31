namespace SmartStudyAI.Backend.Models
{
    public class DocumentUploadRequest
    {
        // This will be handled by FormFile in controller
    }

    public class DocumentChecklistResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string UploadedAt { get; set; } = string.Empty;
        public AssignmentChecklistResponse Checklist { get; set; } = new AssignmentChecklistResponse();
    }
}
