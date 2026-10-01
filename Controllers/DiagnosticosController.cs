using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Lista de valores permitidos para el CHECK de gravedad
        private readonly string[] _gravedadesValidas = { "leve", "moderada", "grave", "critica" };

        public DiagnosticosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODOS
        // GET: api/Diagnosticos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiagnosticoResponseDto>>> GetDiagnosticos()
        {
            var diagnosticos = await _context.Diagnosticos
                .Include(d => d.IdMecanicoNavigation)
                    .ThenInclude(m => m.IdUsuarioNavigation)
                .Select(d => new DiagnosticoResponseDto
                {
                    id_diagnostico = d.IdDiagnostico,
                    id_orden = d.IdOrden,
                    id_mecanico = d.IdMecanico,
                    nombre_mecanico = $"{d.IdMecanicoNavigation.IdUsuarioNavigation.Nombre} {d.IdMecanicoNavigation.IdUsuarioNavigation.Apellido}".Trim(),
                    descripcion = d.Descripcion,
                    gravedad = d.Gravedad,
                    fecha_diagnostico = d.FechaDiagnostico
                })
                .ToListAsync();

            return Ok(diagnosticos);
        }

        // 2. OBTENER UNO
        // GET: api/Diagnosticos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiagnosticoResponseDto>> GetDiagnostico(int id)
        {
            var diagnostico = await _context.Diagnosticos
                .Include(d => d.IdMecanicoNavigation)
                    .ThenInclude(m => m.IdUsuarioNavigation)
                .FirstOrDefaultAsync(d => d.IdDiagnostico == id);

            if (diagnostico == null)
            {
                return NotFound(new { mensaje = "El diagnóstico especificado no existe." });
            }

            var dto = new DiagnosticoResponseDto
            {
                id_diagnostico = diagnostico.IdDiagnostico,
                id_orden = diagnostico.IdOrden,
                id_mecanico = diagnostico.IdMecanico,
                nombre_mecanico = $"{diagnostico.IdMecanicoNavigation.IdUsuarioNavigation.Nombre} {diagnostico.IdMecanicoNavigation.IdUsuarioNavigation.Apellido}".Trim(),
                descripcion = diagnostico.Descripcion,
                gravedad = diagnostico.Gravedad,
                fecha_diagnostico = diagnostico.FechaDiagnostico
            };

            return Ok(dto);
        }

        // 3. CREAR
        // POST: api/Diagnosticos
        [HttpPost]
        public async Task<ActionResult<DiagnosticoResponseDto>> CreateDiagnostico([FromBody] DiagnosticoCreateDto dto)
        {
            // Validar existencia de la orden de trabajo
            var ordenExiste = await _context.OrdenesTrabajos.AnyAsync(o => o.IdOrden == dto.id_orden);
            if (!ordenExiste)
            {
                return BadRequest(new { mensaje = "La orden de trabajo especificada no existe." });
            }

            // Validar existencia y estado del mecánico
            var mecanico = await _context.Mecanicos.FindAsync(dto.id_mecanico);
            if (mecanico == null || mecanico.Activo == false)
            {
                return BadRequest(new { mensaje = "El mecánico especificado no existe o está inactivo." });
            }

            // Validar restricción CHECK de gravedad si viene definida
            if (!string.IsNullOrEmpty(dto.gravedad) && !_gravedadesValidas.Contains(dto.gravedad.Trim().ToLower()))
            {
                return BadRequest(new { mensaje = "La gravedad debe ser uno de los siguientes valores: 'leve', 'moderada', 'grave', 'critica'." });
            }

            var diagnostico = new Diagnostico
            {
                IdOrden = dto.id_orden,
                IdMecanico = dto.id_mecanico,
                Descripcion = dto.descripcion.Trim(),
                Gravedad = dto.gravedad?.Trim().ToLower(),
                FechaDiagnostico = DateTime.Now
            };

            _context.Diagnosticos.Add(diagnostico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDiagnostico),
                new { id = diagnostico.IdDiagnostico },
                new { mensaje = "Diagnóstico registrado correctamente.", id_diagnostico = diagnostico.IdDiagnostico }
            );
        }

        // 4. ACTUALIZAR
        // PUT: api/Diagnosticos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiagnostico(int id, [FromBody] DiagnosticoUpdateDto dto)
        {
            var diagnostico = await _context.Diagnosticos.FindAsync(id);
            if (diagnostico == null)
            {
                return NotFound(new { mensaje = "El diagnóstico a actualizar no existe." });
            }

            // Validar existencia de la orden de trabajo
            var ordenExiste = await _context.OrdenesTrabajos.AnyAsync(o => o.IdOrden == dto.id_orden);
            if (!ordenExiste)
            {
                return BadRequest(new { mensaje = "La orden de trabajo especificada no existe." });
            }

            // Validar existencia y estado del mecánico
            var mecanico = await _context.Mecanicos.FindAsync(dto.id_mecanico);
            if (mecanico == null || mecanico.Activo == false)
            {
                return BadRequest(new { mensaje = "El mecánico especificado no existe o está inactivo." });
            }

            // Validar restricción CHECK de gravedad si viene definida
            if (!string.IsNullOrEmpty(dto.gravedad) && !_gravedadesValidas.Contains(dto.gravedad.Trim().ToLower()))
            {
                return BadRequest(new { mensaje = "La gravedad debe ser uno de los siguientes valores: 'leve', 'moderada', 'grave', 'critica'." });
            }

            diagnostico.IdOrden = dto.id_orden;
            diagnostico.IdMecanico = dto.id_mecanico;
            diagnostico.Descripcion = dto.descripcion.Trim();
            diagnostico.Gravedad = dto.gravedad?.Trim().ToLower();

            _context.Entry(diagnostico).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Diagnóstico actualizado correctamente." });
        }
    }
}