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
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles(){
            var roles = await _context.Roles
                .Select(r => new RoleResponseDto
                {
                    id_rol = r.IdRol,
                    nombre = r.Nombre ?? string.Empty,
                    descripcion = r.Descripcion ?? string.Empty
                })
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleResponseDto>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { mensaje = "El rol especificado no existe." });
            }

            return Ok(new RoleResponseDto
            {
                id_rol = role.IdRol,
                nombre = role.Nombre ?? string.Empty,
                descripcion = role.Descripcion ?? string.Empty
            });
        }

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<RoleResponseDto>> CreateRole([FromBody] RoleCreateUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.nombre))
            {
                return BadRequest(new { mensaje = "El nombre del rol es obligatorio." });
            }

            var nuevoRol = new Role
            {
                Nombre = dto.nombre,
                Descripcion = dto.descripcion
            };

            _context.Roles.Add(nuevoRol);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRole), new { id = nuevoRol.IdRol }, new RoleResponseDto
            {
                id_rol = nuevoRol.IdRol,
                nombre = nuevoRol.Nombre,
                descripcion = nuevoRol.Descripcion
            });
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleCreateUpdateDto dto)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { mensaje = "El rol a actualizar no existe." });
            }

            if (string.IsNullOrWhiteSpace(dto.nombre))
            {
                return BadRequest(new { mensaje = "El nombre del rol es obligatorio." });
            }

            role.Nombre = dto.nombre;
            role.Descripcion = dto.descripcion;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol actualizado correctamente." });
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { mensaje = "El rol a eliminar no existe." });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Rol eliminado correctamente." });
        }
    }
}