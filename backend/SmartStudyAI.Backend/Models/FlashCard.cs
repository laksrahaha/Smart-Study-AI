using System.Text.Json.Serialization;

namespace SmartStudyAI.Backend.Models
{
    public class FlashCard
    {
        [JsonPropertyName("front")]
        public string Front { get; set; } = string.Empty;

        [JsonPropertyName("back")]
        public string Back { get; set; } = string.Empty;
    }
}
