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
            // Cargar el usuario, su Rol y los Permisos asociados al Rol
            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                    .ThenInclude(r => r.IdPermisos)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null || usuario.PasswordHash != request.Password)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }

            // 1. Obtener nombres de permisos heredados del Rol
            var permisosRol = usuario.IdRolNavigation?.IdPermisos
                .Select(p => p.Nombre)
                .ToList() ?? new List<string>();

            // 2. Obtener nombres de permisos asignados directamente al usuario
            var permisosDirectos = await _context.UsuariosPermisos
                .Where(up => up.IdUsuario == usuario.IdUsuario)
                .Select(up => up.IdPermisoNavigation.Nombre)
                .ToListAsync();

            // 3. Unir ambos listados sin duplicados
            var listaPermisos = permisosRol.Union(permisosDirectos).Distinct().ToList();

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

            // Retornar la respuesta con la lista de permisos/módulos
            return Ok(new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Usuario = usuario.Nombre ?? string.Empty,
                Rol = usuario.IdRolNavigation?.Nombre ?? "Sin Rol",
                Expiracion = expiracion,
                Permisos = listaPermisos
            });
        }
    }
}