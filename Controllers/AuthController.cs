using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            // Busca al usuario en la BD (ajusta la propiedad 'Email' o 'Password' si en tus modelos se llaman distinto)
            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // Validar existencia y contraseña
            if (usuario == null || usuario.PasswordHash != request.Password)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }

            // Generar claims para el JWT
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre ?? string.Empty),
                new Claim(ClaimTypes.Email, usuario.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, usuario.IdRolNavigation?.Nombre ?? "Usuario")
            };

            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddHours(8);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expiracion,
                signingCredentials: creds
            );

            return Ok(new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Usuario = usuario.Nombre ?? string.Empty,
                Rol = usuario.IdRolNavigation?.Nombre ?? "Sin Rol",
                Expiracion = expiracion
            });
        }
    }
}