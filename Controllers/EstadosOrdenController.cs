using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadosOrdenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstadosOrdenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODOS
        // GET: api/EstadosOrden
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstadoOrdenResponseDto>>> GetEstadosOrden()
        {
            var estados = await _context.EstadosOrdens
                .Select(e => new EstadoOrdenResponseDto
                {
                    id_estado = e.IdEstado,
                    nombre = e.Nombre
                })
                .ToListAsync();

            return Ok(estados);
        }

        // 2. OBTENER UNO
        // GET: api/EstadosOrden/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EstadoOrdenResponseDto>> GetEstadoOrden(int id)
        {
            var estado = await _context.EstadosOrdens.FindAsync(id);

            if (estado == null)
            {
                return NotFound(new { mensaje = "El estado de orden especificado no existe." });
            }

            var dto = new EstadoOrdenResponseDto
            {
                id_estado = estado.IdEstado,
                nombre = estado.Nombre
            };

            return Ok(dto);
        }

        // 3. CREAR
        // POST: api/EstadosOrden
        [HttpPost]
        public async Task<ActionResult<EstadoOrdenResponseDto>> CreateEstadoOrden([FromBody] EstadoOrdenCreateDto dto)
        {
            // Validar restricción UNIQUE: el nombre no debe duplicarse
            var nombreExiste = await _context.EstadosOrdens
                .AnyAsync(e => e.Nombre.ToLower() == dto.nombre.Trim().ToLower());

            if (nombreExiste)
            {
                return BadRequest(new { mensaje = "Ya existe un estado de orden registrado con ese nombre." });
            }

            var estado = new EstadosOrden
            {
                Nombre = dto.nombre.Trim()
            };

            _context.EstadosOrdens.Add(estado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEstadoOrden),
                new { id = estado.IdEstado },
                new { mensaje = "Estado de orden registrado correctamente.", id_estado = estado.IdEstado }
            );
        }

        // 4. ACTUALIZAR
        // PUT: api/EstadosOrden/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEstadoOrden(int id, [FromBody] EstadoOrdenUpdateDto dto)
        {
            var estado = await _context.EstadosOrdens.FindAsync(id);

            if (estado == null)
            {
                return NotFound(new { mensaje = "El estado de orden a actualizar no existe." });
            }

            // Validar que el nuevo nombre no pertenezca a OTRO estado
            var nombreExiste = await _context.EstadosOrdens
                .AnyAsync(e => e.Nombre.ToLower() == dto.nombre.Trim().ToLower() && e.IdEstado != id);

            if (nombreExiste)
            {
                return BadRequest(new { mensaje = "Ya existe otro estado de orden registrado con ese nombre." });
            }

            estado.Nombre = dto.nombre.Trim();

            _context.Entry(estado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado de orden actualizado correctamente." });
        }
    }
}