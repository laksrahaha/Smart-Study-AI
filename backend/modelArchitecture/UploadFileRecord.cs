namespace SmartStudyAI.Backend.Models
{
    public class UploadFileRecord
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}