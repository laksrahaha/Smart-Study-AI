namespace SmartStudyAI.Backend.Models
{
    public class AssignmentChecklistRequest
    {
        public string AssignmentContent { get; set; } = string.Empty;
    }

    public class ChecklistItem
    {
        public int Id { get; set; }
        public string Task { get; set; } = string.Empty;
        public string Guide { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public int Priority { get; set; } // 1 = High, 2 = Medium, 3 = Low
    }

    public class AssignmentChecklistResponse
    {
        public string AssignmentTitle { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public List<ChecklistItem> Checklist { get; set; } = new List<ChecklistItem>();
        public string OverallGuidance { get; set; } = string.Empty;
        public int EstimatedTimeMinutes { get; set; }
    }
}
