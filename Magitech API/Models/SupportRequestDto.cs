using System.ComponentModel.DataAnnotations;

namespace MagitechAPI.Models
{
    public class SupportRequestDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mensagem é obrigatória")]
        public string Mensagem { get; set; } = string.Empty;
    }
}
