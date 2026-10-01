using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensSesionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TokensSesionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TokensSesion/usuario/5
        // Ohechauka opaite tokens de un usuario específico
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<TokenSesionResponseDto>>> GetTokensPorUsuario(int idUsuario)
        {
            var tokens = await _context.TokensSesions
                .Where(t => t.IdUsuario == idUsuario)
                .Select(t => new TokenSesionResponseDto
                {
                    id_token = t.IdToken,
                    id_usuario = t.IdUsuario,
                    token = t.Token,
                    fecha_emision = t.FechaEmision,
                    fecha_expiracion = t.FechaExpiracion,
                    revocado = t.Revocado
                })
                .ToListAsync();

            return Ok(tokens);
        }

        // POST: api/TokensSesion/crear
        // Omoĩ token pyahu base de datos-pe (por ejemplo, Login me)
        [HttpPost("crear")]
        public async Task<ActionResult<TokenSesionResponseDto>> RegistrarToken([FromBody] TokenSesionCreateDto dto)
        {
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.id_usuario && u.Activo == true);
            if (!usuarioExiste)
            {
                return BadRequest(new { mensaje = "El usuario no existe o está inactivo." });
            }

            var tokenSesion = new TokensSesion
            {
                IdUsuario = dto.id_usuario,
                Token = dto.token,
                FechaEmision = DateTime.Now,
                FechaExpiracion = dto.fecha_expiracion,
                Revocado = false
            };

            _context.TokensSesions.Add(tokenSesion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Token de sesión registrado correctamente.", id_token = tokenSesion.IdToken });
        }

        // POST: api/TokensSesion/revocar
        // Ojapo revocación de un token (Logout)
        [HttpPost("revocar")]
        public async Task<IActionResult> RevocarToken([FromBody] RevocarTokenDto dto)
        {
            var tokenSesion = await _context.TokensSesions
                .FirstOrDefaultAsync(t => t.Token == dto.token);

            if (tokenSesion == null)
            {
                return NotFound(new { mensaje = "El token no fue encontrado." });
            }

            tokenSesion.Revocado = true;
            _context.Entry(tokenSesion).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Token revocado (sesión cerrada) correctamente." });
        }

        // POST: api/TokensSesion/revocar-todos/5
        // Orevoca opaite tokens de un usuario (Cierra sesión en todos los dispositivos)
        [HttpPost("revocar-todos/{idUsuario}")]
        public async Task<IActionResult> RevocarTodosTokensUsuario(int idUsuario)
        {
            var tokensActivos = await _context.TokensSesions
                .Where(t => t.IdUsuario == idUsuario && (t.Revocado == false || t.Revocado == null))
                .ToListAsync();

            if (!tokensActivos.Any())
            {
                return Ok(new { mensaje = "No hay sesiones activas para este usuario." });
            }

            foreach (var token in tokensActivos)
            {
                token.Revocado = true;
                _context.Entry(token).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Se han cerrado todas las sesiones del usuario correctamente." });
        }
    }
}