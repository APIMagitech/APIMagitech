using System.Net;
using System.Net.Mail;
using System.Text;
using MagitechAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagitechAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly string? _gmailUser;
        private readonly string? _gmailPass;

        public EmailController(ILogger<EmailController> logger)
        {
            _logger = logger;
            _gmailUser = Environment.GetEnvironmentVariable("GMAIL_USER");
            _gmailPass = Environment.GetEnvironmentVariable("GMAIL_PASS");
        }

        private SmtpClient CreateClient()
        {
            if (string.IsNullOrWhiteSpace(_gmailUser) || string.IsNullOrWhiteSpace(_gmailPass))
            {
                throw new InvalidOperationException("Variaveis GMAIL_USER/GMAIL_PASS nao configuradas.");
            }

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_gmailUser, _gmailPass)
            };

            return client;
        }

        // POST: /api/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Email e obrigatorio"
                });
            }

            try
            {
                var random = new Random();
                var recoveryCode = random.Next(100000, 1000000);

                var sb = new StringBuilder();
                sb.AppendLine("Ola!");
                sb.AppendLine();
                sb.AppendLine("Voce solicitou a recuperacao de senha da Magitech Studios.");
                sb.AppendLine();
                sb.AppendLine($"Seu codigo de recuperacao e: {recoveryCode}");
                sb.AppendLine();
                sb.AppendLine("Este codigo e valido por 30 minutos.");
                sb.AppendLine();
                sb.AppendLine("Se voce nao solicitou esta recuperacao, ignore este email.");
                sb.AppendLine();
                sb.AppendLine("Atenciosamente,");
                sb.AppendLine("Equipe Magitech Studios");

                using var client = CreateClient();
                using var mail = new MailMessage(_gmailUser!, dto.Email)
                {
                    Subject = "Recuperacao de Senha - Magitech Studios",
                    Body = sb.ToString(),
                    IsBodyHtml = false
                };

                await client.SendMailAsync(mail);

                return Ok(new
                {
                    success = true,
                    message = "Email de recuperacao enviado com sucesso!",
                    code = recoveryCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de recuperacao");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao enviar email de recuperacao.",
                    error = ex.Message
                });
            }
        }

        // POST: /api/send-feedback
        [HttpPost("send-feedback")]
        public async Task<IActionResult> SendFeedback([FromBody] FeedbackRequestDto dto)
        {
            try
            {
                using var client = CreateClient();

                var coursePart = string.IsNullOrWhiteSpace(dto.CourseId)
                    ? string.Empty
                    : dto.CourseId!.ToUpperInvariant();

                var sb = new StringBuilder();
                sb.Append("FEEDBACK - CURSO: ");
                sb.AppendLine(coursePart);
                sb.AppendLine("========================================");
                sb.AppendLine();

                if (dto.Responses != null && dto.Responses.Count > 0)
                {
                    for (var i = 0; i < dto.Responses.Count; i++)
                    {
                        var item = dto.Responses[i];
                        sb.AppendLine($"{i + 1}. {item.Question}");
                        if (!string.IsNullOrWhiteSpace(item.Subtitle))
                        {
                            sb.AppendLine($"   {item.Subtitle}");
                        }

                        sb.AppendLine($"   Resposta: {item.Answer}");
                        sb.AppendLine();
                    }
                }

                var timestamp = dto.Timestamp ?? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                sb.AppendLine("========================================");
                sb.AppendLine($"Data: {timestamp}");

                using var mail = new MailMessage(_gmailUser!, dto.RecipientEmail)
                {
                    Subject = $"Feedback - {dto.CourseId}",
                    Body = sb.ToString(),
                    IsBodyHtml = false
                };

                await client.SendMailAsync(mail);

                return Ok(new
                {
                    success = true,
                    message = "E-mail enviado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de feedback");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao enviar e-mail.",
                    error = ex.Message
                });
            }
        }

        // POST: /api/enviar-suporte
        [HttpPost("enviar-suporte")]
        public async Task<IActionResult> EnviarSuporte([FromBody] SupportRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Por favor, preencha todos os campos"
                });
            }

            try
            {
                using var client = CreateClient();

                var dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                var sb = new StringBuilder();
                sb.AppendLine("Nova mensagem de suporte - Magitech");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine($"Nome: {dto.Nome}");
                sb.AppendLine($"Email: {dto.Email}");
                sb.AppendLine();
                sb.AppendLine("Mensagem:");
                sb.AppendLine(dto.Mensagem);
                sb.AppendLine();
                sb.AppendLine($"Data/Hora: {dataHora}");

                using var mail = new MailMessage(_gmailUser!, _gmailUser!)
                {
                    Subject = $"Suporte Magitech - {dto.Nome}",
                    Body = sb.ToString(),
                    IsBodyHtml = false
                };

                mail.ReplyToList.Add(new MailAddress(dto.Email));

                await client.SendMailAsync(mail);

                _logger.LogInformation("Email de suporte enviado para {Email}", _gmailUser);

                return Ok(new
                {
                    success = true,
                    message = "Mensagem enviada com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de suporte");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao enviar mensagem. Tente novamente.",
                    error = ex.Message
                });
            }
        }
    }
}
