using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeuSiteAPI.Data;
using MeuSiteAPI.Models;
using BCrypt.Net;

namespace MeuSiteAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(AppDbContext context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/usuarios/cadastro
        [HttpPost("cadastro")]
        public async Task<ActionResult> Cadastro([FromBody] CadastroDto dto)
        {
            try
            {
                // Verificar se email já existe
                var emailExiste = await _context.Usuarios
                    .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (emailExiste)
                {
                    return Conflict(new { erro = "Este email já está cadastrado" });
                }

                // Criptografar senha
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

                // Criar usuário
                var usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nome = dto.Nome.Trim(),
                    Sobrenome = dto.Sobrenome.Trim(),
                    Email = dto.Email.ToLower().Trim(),
                    SenhaHash = senhaHash,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário cadastrado: {Email}", usuario.Email);

                return Ok(new
                {
                    mensagem = "Usuário cadastrado com sucesso!",
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nome,
                        usuario.Sobrenome,
                        usuario.Email,
                        usuario.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuário");
                return StatusCode(500, new { erro = "Erro interno do servidor" });
            }
        }

        // POST: api/usuarios/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                // Buscar usuário por email
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

                if (usuario == null)
                {
                    return Unauthorized(new { erro = "Email ou senha inválidos" });
                }

                // Verificar senha
                bool senhaCorreta = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash);

                if (!senhaCorreta)
                {
                    return Unauthorized(new { erro = "Email ou senha inválidos" });
                }

                _logger.LogInformation("Login realizado: {Email}", usuario.Email);

                return Ok(new
                {
                    mensagem = "Login realizado com sucesso!",
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nome,
                        usuario.Sobrenome,
                        usuario.Email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login");
                return StatusCode(500, new { erro = "Erro interno do servidor" });
            }
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult> ListarTodos()
        {
            try
            {
                var usuarios = await _context.Usuarios
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Sobrenome,
                        u.Email,
                        u.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new
                {
                    total = usuarios.Count,
                    usuarios
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuários");
                return StatusCode(500, new { erro = "Erro interno do servidor" });
            }
        }

        // GET: api/usuarios/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> BuscarPorId(Guid id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Where(u => u.Id == id)
                    .Select(u => new
                    {
                        u.Id,
                        u.Nome,
                        u.Sobrenome,
                        u.Email,
                        u.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound(new { erro = "Usuário não encontrado" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário");
                return StatusCode(500, new { erro = "Erro interno do servidor" });
            }
        }

        // GET: api/usuarios/health
        [HttpGet("health")]
        public ActionResult Health()
        {
            return Ok(new
            {
                status = "API funcionando!",
                timestamp = DateTime.UtcNow
            });
        }
    }
}