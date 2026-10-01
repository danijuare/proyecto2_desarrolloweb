using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MecanicosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MecanicosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODOS
        // GET: api/Mecanicos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MecanicoResponseDto>>> GetMecanicos()
        {
            var mecanicos = await _context.Mecanicos
                .Include(m => m.IdUsuarioNavigation)
                .Where(m => m.Activo == true)
                .Select(m => new MecanicoResponseDto
                {
                    id_mecanico = m.IdMecanico,
                    id_usuario = m.IdUsuario,
                    nombre_completo = $"{m.IdUsuarioNavigation.Nombre} {m.IdUsuarioNavigation.Apellido}".Trim(),
                    email = m.IdUsuarioNavigation.Email,
                    especialidad = m.Especialidad,
                    activo = m.Activo ?? true
                })
                .ToListAsync();

            return Ok(mecanicos);
        }

        // 2. OBTENER UNO
        // GET: api/Mecanicos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MecanicoResponseDto>> GetMecanico(int id)
        {
            var mecanico = await _context.Mecanicos
                .Include(m => m.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.IdMecanico == id && m.Activo == true);

            if (mecanico == null)
            {
                return NotFound(new { mensaje = "El mecánico no existe o está desactivado." });
            }

            var dto = new MecanicoResponseDto
            {
                id_mecanico = mecanico.IdMecanico,
                id_usuario = mecanico.IdUsuario,
                nombre_completo = $"{mecanico.IdUsuarioNavigation.Nombre} {mecanico.IdUsuarioNavigation.Apellido}".Trim(),
                email = mecanico.IdUsuarioNavigation.Email,
                especialidad = mecanico.Especialidad,
                activo = mecanico.Activo ?? true
            };

            return Ok(dto);
        }

        // 3. CREAR
        // POST: api/Mecanicos
        [HttpPost]
        public async Task<ActionResult<MecanicoResponseDto>> CreateMecanico([FromBody] MecanicoCreateDto dto)
        {
            // Validar que el usuario exista y esté activo
            var usuario = await _context.Usuarios.FindAsync(dto.id_usuario);
            if (usuario == null || usuario.Activo == false)
            {
                return BadRequest(new { mensaje = "El usuario especificado no existe o está inactivo." });
            }

            // Validar restricción UNIQUE: un usuario solo puede ser asignado a un mecánico
            var usuarioExisteEnMecanicos = await _context.Mecanicos
                .AnyAsync(m => m.IdUsuario == dto.id_usuario);

            if (usuarioExisteEnMecanicos)
            {
                return BadRequest(new { mensaje = "El usuario especificado ya está registrado como mecánico." });
            }

            var mecanico = new Mecanico
            {
                IdUsuario = dto.id_usuario,
                Especialidad = dto.especialidad?.Trim(),
                Activo = true
            };

            _context.Mecanicos.Add(mecanico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMecanico),
                new { id = mecanico.IdMecanico },
                new { mensaje = "Mecánico registrado correctamente.", id_mecanico = mecanico.IdMecanico }
            );
        }

        // 4. ACTUALIZAR
        // PUT: api/Mecanicos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMecanico(int id, [FromBody] MecanicoUpdateDto dto)
        {
            var mecanico = await _context.Mecanicos.FindAsync(id);

            if (mecanico == null || mecanico.Activo == false)
            {
                return NotFound(new { mensaje = "El mecánico a actualizar no existe o está desactivado." });
            }

            // Validar que el usuario exista y esté activo
            var usuario = await _context.Usuarios.FindAsync(dto.id_usuario);
            if (usuario == null || usuario.Activo == false)
            {
                return BadRequest(new { mensaje = "El usuario especificado no existe o está inactivo." });
            }

            // Validar que el usuario no esté asignado a OTRO mecánico
            var usuarioExisteEnMecanicos = await _context.Mecanicos
                .AnyAsync(m => m.IdUsuario == dto.id_usuario && m.IdMecanico != id);

            if (usuarioExisteEnMecanicos)
            {
                return BadRequest(new { mensaje = "El usuario especificado ya está asignado a otro mecánico." });
            }

            mecanico.IdUsuario = dto.id_usuario;
            mecanico.Especialidad = dto.especialidad?.Trim();

            _context.Entry(mecanico).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Mecánico actualizado correctamente." });
        }

        // 5. ELIMINAR (BORRADO LÓGICO)
        // DELETE: api/Mecanicos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMecanico(int id)
        {
            var mecanico = await _context.Mecanicos.FindAsync(id);

            if (mecanico == null)
            {
                return NotFound(new { mensaje = "El mecánico a desactivar no existe." });
            }

            // Desactivación lógica
            mecanico.Activo = false;
            _context.Entry(mecanico).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Mecánico desactivado correctamente." });
        }
    }
}