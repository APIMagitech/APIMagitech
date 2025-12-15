using System.ComponentModel.DataAnnotations;

namespace MagitechAPI.Models
{
    public class FeedbackResponseItemDto
    {
        public string Question { get; set; } = string.Empty;
        public string? Subtitle { get; set; }
        public string Answer { get; set; } = string.Empty;
    }

    public class FeedbackRequestDto
    {
        public string? CourseId { get; set; }

        [Required(ErrorMessage = "Email do destinatário é obrigatório")]
        public string RecipientEmail { get; set; } = string.Empty;

        public List<FeedbackResponseItemDto> Responses { get; set; } = new();

        public string? Timestamp { get; set; }
    }
}
