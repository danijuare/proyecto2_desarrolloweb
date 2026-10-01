using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesPermisosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesPermisosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RolesPermisos/rol/5
        // Obtiene todos los permisos asociados a un rol específico
        [HttpGet("rol/{idRol}")]
        public async Task<ActionResult<RoleConPermisosDto>> GetPermisosPorRol(int idRol)
        {
            var rol = await _context.Roles
                .Include(r => r.IdPermisos)
                .FirstOrDefaultAsync(r => r.IdRol == idRol);

            if (rol == null)
            {
                return NotFound(new { mensaje = "El rol especificado no existe." });
            }

            var respuesta = new RoleConPermisosDto
            {
                id_rol = rol.IdRol,
                nombre_rol = rol.Nombre,
                permisos = rol.IdPermisos
                    .Where(p => p.Condicion == 1) // Solo incluye permisos activos
                    .Select(p => new PermisoResponseDto
                    {
                        id_permiso = p.IdPermiso,
                        codigo = p.Codigo,
                        nombre = p.Nombre,
                        descripcion = p.Descripcion,
                    }).ToList()
            };

            return Ok(respuesta);
        }

        // POST: api/RolesPermisos/asignar
        // Asigna un solo permiso a un rol
        [HttpPost("asignar")]
        public async Task<IActionResult> AsignarPermiso([FromBody] RolePermisoDto dto)
        {
            var rol = await _context.Roles.Include(r => r.IdPermisos).FirstOrDefaultAsync(r => r.IdRol == dto.id_rol);
            if (rol == null) return NotFound(new { mensaje = "El rol especificado no existe." });

            var permiso = await _context.Permisos.FindAsync(dto.id_permiso);
            if (permiso == null) return NotFound(new { mensaje = "El permiso especificado no existe." });

            if (rol.IdPermisos.Any(p => p.IdPermiso == dto.id_permiso))
            {
                return BadRequest(new { mensaje = "El rol ya tiene asignado este permiso." });
            }

            rol.IdPermisos.Add(permiso);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Permiso asignado al rol correctamente." });
        }

        // POST: api/RolesPermisos/sincronizar
        // Reemplaza todos los permisos de un rol por una nueva lista de IDs
        [HttpPost("sincronizar")]
        public async Task<IActionResult> SincronizarPermisos([FromBody] AsignarPermisosRoleDto dto)
        {
            var rol = await _context.Roles.Include(r => r.IdPermisos).FirstOrDefaultAsync(r => r.IdRol == dto.id_rol);
            if (rol == null) return NotFound(new { mensaje = "El rol especificado no existe." });

            var permisosNuevos = await _context.Permisos
                .Where(p => dto.ids_permisos.Contains(p.IdPermiso))
                .ToListAsync();

            // Limpia los actuales y asigna la lista enviada
            rol.IdPermisos.Clear();
            foreach (var permiso in permisosNuevos)
            {
                rol.IdPermisos.Add(permiso);
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Permisos del rol actualizados correctamente." });
        }

        // DELETE: api/RolesPermisos/desasignar
        // Remueve la relación entre un rol y un permiso
        [HttpDelete("desasignar")]
        public async Task<IActionResult> DesasignarPermiso([FromBody] RolePermisoDto dto)
        {
            var rol = await _context.Roles.Include(r => r.IdPermisos).FirstOrDefaultAsync(r => r.IdRol == dto.id_rol);
            if (rol == null) return NotFound(new { mensaje = "El rol especificado no existe." });

            var permiso = rol.IdPermisos.FirstOrDefault(p => p.IdPermiso == dto.id_permiso);
            if (permiso == null)
            {
                return NotFound(new { mensaje = "El rol no tiene asignado dicho permiso." });
            }

            rol.IdPermisos.Remove(permiso);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Permiso desasignado del rol correctamente." });
        }
    }
}