using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosPermisosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosPermisosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UsuariosPermisos/usuario/5
        // Obtiene los permisos directos asignados a un usuario específico
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<UsuarioConPermisosDto>> GetPermisosPorUsuario(int idUsuario)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuariosPermisos)
                    .ThenInclude(up => up.IdPermisoNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.Activo == true);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario especificado no existe o está inactivo." });
            }

            var respuesta = new UsuarioConPermisosDto
            {
                id_usuario = usuario.IdUsuario,
                nombre_completo = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                email = usuario.Email,
                permisos_directos = usuario.UsuariosPermisos
                    .Where(up => up.IdPermisoNavigation.Condicion == 1) // Solo permisos activos
                    .Select(up => new PermisoUsuarioDetalleDto
                    {
                        id_permiso = up.IdPermiso,
                        codigo = up.IdPermisoNavigation.Codigo,
                        nombre = up.IdPermisoNavigation.Nombre,
                        descripcion = up.IdPermisoNavigation.Descripcion,
                        tipo = up.Tipo
                    }).ToList()
            };

            return Ok(respuesta);
        }

        // POST: api/UsuariosPermisos/asignar
        // Asigna un permiso (conceder/denegar) o actualiza el tipo si ya existe
        [HttpPost("asignar")]
        public async Task<IActionResult> AsignarOActualizarPermiso([FromBody] UsuarioPermisoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios.FindAsync(dto.id_usuario);
            if (usuario == null || usuario.Activo == false)
            {
                return NotFound(new { mensaje = "El usuario especificado no existe o está inactivo." });
            }

            var permiso = await _context.Permisos.FindAsync(dto.id_permiso);
            if (permiso == null)
            {
                return NotFound(new { mensaje = "El permiso especificado no existe." });
            }

            var usuarioPermisoExistente = await _context.UsuariosPermisos
                .FirstOrDefaultAsync(up => up.IdUsuario == dto.id_usuario && up.IdPermiso == dto.id_permiso);

            if (usuarioPermisoExistente != null)
            {
                // Si la asignación ya existía, actualiza el tipo ('conceder' o 'denegar')
                usuarioPermisoExistente.Tipo = dto.tipo.ToLower();
                _context.Entry(usuarioPermisoExistente).State = EntityState.Modified;
            }
            else
            {
                // Crea el nuevo registro
                var nuevoRegistro = new UsuariosPermiso
                {
                    IdUsuario = dto.id_usuario,
                    IdPermiso = dto.id_permiso,
                    Tipo = dto.tipo.ToLower()
                };
                _context.UsuariosPermisos.Add(nuevoRegistro);
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Permiso seteados como '{dto.tipo.ToLower()}' para el usuario correctamente." });
        }

        // DELETE: api/UsuariosPermisos/desasignar
        // Remueve la asignación directa de un permiso para un usuario
        [HttpDelete("desasignar")]
        public async Task<IActionResult> DesasignarPermiso([FromBody] UsuarioPermisoEliminarDto dto)
        {
            var usuarioPermiso = await _context.UsuariosPermisos
                .FirstOrDefaultAsync(up => up.IdUsuario == dto.id_usuario && up.IdPermiso == dto.id_permiso);

            if (usuarioPermiso == null)
            {
                return NotFound(new { mensaje = "El usuario no tiene una asignación directa para este permiso." });
            }

            _context.UsuariosPermisos.Remove(usuarioPermiso);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Permiso directo removido del usuario correctamente." });
        }
    }
}