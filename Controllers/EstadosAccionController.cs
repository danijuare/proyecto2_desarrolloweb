using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosAccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstadosAccionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODOS
        // GET: api/EstadosAccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstadoAccionResponseDto>>> GetEstadosAccion()
        {
            var estados = await _context.EstadosAccions
                .Select(e => new EstadoAccionResponseDto
                {
                    id_estado = e.IdEstado,
                    nombre = e.Nombre
                })
                .ToListAsync();

            return Ok(estados);
        }

        // 2. OBTENER UNO
        // GET: api/EstadosAccion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EstadoAccionResponseDto>> GetEstadoAccion(int id)
        {
            var estado = await _context.EstadosAccions.FindAsync(id);

            if (estado == null)
            {
                return NotFound(new { mensaje = "El estado de acción especificado no existe." });
            }

            var dto = new EstadoAccionResponseDto
            {
                id_estado = estado.IdEstado,
                nombre = estado.Nombre
            };

            return Ok(dto);
        }

        // 3. CREAR
        // POST: api/EstadosAccion
        [HttpPost]
        public async Task<ActionResult<EstadoAccionResponseDto>> CreateEstadoAccion([FromBody] EstadoAccionCreateDto dto)
        {
            // Validar restricción UNIQUE: el nombre no debe duplicarse
            var nombreExiste = await _context.EstadosAccions
                .AnyAsync(e => e.Nombre.ToLower() == dto.nombre.Trim().ToLower());

            if (nombreExiste)
            {
                return BadRequest(new { mensaje = "Ya existe un estado de acción registrado con ese nombre." });
            }

            var estado = new EstadosAccion
            {
                Nombre = dto.nombre.Trim()
            };

            _context.EstadosAccions.Add(estado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEstadoAccion),
                new { id = estado.IdEstado },
                new { mensaje = "Estado de acción registrado correctamente.", id_estado = estado.IdEstado }
            );
        }

        // 4. ACTUALIZAR
        // PUT: api/EstadosAccion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstadoAccion(int id, [FromBody] EstadoAccionUpdateDto dto)
        {
            var estado = await _context.EstadosAccions.FindAsync(id);

            if (estado == null)
            {
                return NotFound(new { mensaje = "El estado de acción a actualizar no existe." });
            }

            // Validar que el nuevo nombre no pertenezca a OTRO estado
            var nombreExiste = await _context.EstadosAccions
                .AnyAsync(e => e.Nombre.ToLower() == dto.nombre.Trim().ToLower() && e.IdEstado != id);

            if (nombreExiste)
            {
                return BadRequest(new { mensaje = "Ya existe otro estado de acción registrado con ese nombre." });
            }

            estado.Nombre = dto.nombre.Trim();

            _context.Entry(estado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado de acción actualizado correctamente." });
        }
    }
}