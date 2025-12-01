using System.ComponentModel.DataAnnotations;

namespace MeuSiteAPI.Models
{
    public class CadastroDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MinLength(2, ErrorMessage = "Nome deve ter no mínimo 2 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        [MinLength(2, ErrorMessage = "Sobrenome deve ter no mínimo 2 caracteres")]
        public string Sobrenome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Senha { get; set; } = string.Empty;
    }
}