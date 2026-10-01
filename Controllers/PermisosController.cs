using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PermisosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Permisos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermisoResponseDto>>> GetPermisos()
        {
            var permisos = await _context.Permisos
                .Select(p => new PermisoResponseDto
                {
                    id_permiso = p.IdPermiso,
                    codigo = p.Codigo ?? string.Empty,
                    nombre = p.Nombre ?? string.Empty,
                    descripcion = p.Descripcion
                })
                .ToListAsync();

            return Ok(permisos);
        }

        // GET: api/Permisos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PermisoResponseDto>> GetPermiso(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);

            if (permiso == null)
            {
                return NotFound(new { mensaje = "El permiso especificado no existe." });
            }

            return Ok(new PermisoResponseDto
            {
                id_permiso = permiso.IdPermiso,
                codigo = permiso.Codigo ?? string.Empty,
                nombre = permiso.Nombre ?? string.Empty,
                descripcion = permiso.Descripcion
            });
        }

        // POST: api/Permisos
        [HttpPost]
        public async Task<ActionResult<PermisoResponseDto>> CreatePermiso([FromBody] PermisoCreateUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.codigo))
            {
                return BadRequest(new { mensaje = "El código del permiso es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(dto.nombre))
            {
                return BadRequest(new { mensaje = "El nombre del permiso es obligatorio." });
            }

            // Validar que el código no se repita (es UNIQUE en BD)
            bool existeCodigo = await _context.Permisos.AnyAsync(p => p.Codigo == dto.codigo);
            if (existeCodigo)
            {
                return BadRequest(new { mensaje = "Ya existe un permiso con el mismo código." });
            }

            var nuevoPermiso = new Permiso
            {
                Codigo = dto.codigo,
                Nombre = dto.nombre,
                Descripcion = dto.descripcion
            };

            _context.Permisos.Add(nuevoPermiso);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPermiso), new { id = nuevoPermiso.IdPermiso }, new PermisoResponseDto
            {
                id_permiso = nuevoPermiso.IdPermiso,
                codigo = nuevoPermiso.Codigo,
                nombre = nuevoPermiso.Nombre,
                descripcion = nuevoPermiso.Descripcion
            });
        }

        // PUT: api/Permisos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermiso(int id, [FromBody] PermisoCreateUpdateDto dto)
        {
            var permiso = await _context.Permisos.FindAsync(id);

            if (permiso == null)
            {
                return NotFound(new { mensaje = "El permiso a actualizar no existe." });
            }

            if (string.IsNullOrWhiteSpace(dto.codigo))
            {
                return BadRequest(new { mensaje = "El código del permiso es obligatorio." });
            }

            if (string.IsNullOrWhiteSpace(dto.nombre))
            {
                return BadRequest(new { mensaje = "El nombre del permiso es obligatorio." });
            }

            // Verificar si se intenta cambiar a un código que ya le pertenece a otro registro
            bool existeCodigo = await _context.Permisos.AnyAsync(p => p.Codigo == dto.codigo && p.IdPermiso != id);
            if (existeCodigo)
            {
                return BadRequest(new { mensaje = "Ya existe otro permiso con el mismo código." });
            }

            permiso.Codigo = dto.codigo;
            permiso.Nombre = dto.nombre;
            permiso.Descripcion = dto.descripcion;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Permiso actualizado correctamente." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermiso(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);

            if (permiso == null)
            {
                return NotFound(new { mensaje = "El permiso a eliminar no existe." });
            }

            // 1. Asignar el valor 0
            permiso.Condicion = 0;

            // 2. Notificar explícitamente a EF Core que el objeto cambió
            _context.Entry(permiso).State = EntityState.Modified;

            // 3. Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Permiso desactivado correctamente." });
        }
    }
}