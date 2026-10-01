using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccionesReparacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccionesReparacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODOS
        // GET: api/AccionesReparacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccionReparacionResponseDto>>> GetAccionesReparacion()
        {
            var acciones = await _context.AccionesReparacions
                .Include(a => a.IdEstadoNavigation)
                .Include(a => a.IdUsuarioAutorizaNavigation)
                .Select(a => new AccionReparacionResponseDto
                {
                    id_accion = a.IdAccion,
                    id_diagnostico = a.IdDiagnostico,
                    descripcion = a.Descripcion,
                    id_estado = a.IdEstado,
                    nombre_estado = a.IdEstadoNavigation.Nombre,
                    id_usuario_autoriza = a.IdUsuarioAutoriza,
                    usuario_autoriza = a.IdUsuarioAutorizaNavigation != null
                        ? $"{a.IdUsuarioAutorizaNavigation.Nombre} {a.IdUsuarioAutorizaNavigation.Apellido}".Trim()
                        : null,
                    fecha_autorizacion = a.FechaAutorizacion,
                    orden_ejecucion = a.OrdenEjecucion,
                    costo_mano_obra = a.CostoManoObra ?? 0,
                    fecha_creacion = a.FechaCreacion,
                    fecha_finalizacion = a.FechaFinalizacion
                })
                .ToListAsync();

            return Ok(acciones);
        }

        // 2. OBTENER UNO
        // GET: api/AccionesReparacion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccionReparacionResponseDto>> GetAccionReparacion(int id)
        {
            var accion = await _context.AccionesReparacions
                .Include(a => a.IdEstadoNavigation)
                .Include(a => a.IdUsuarioAutorizaNavigation)
                .FirstOrDefaultAsync(a => a.IdAccion == id);

            if (accion == null)
            {
                return NotFound(new { mensaje = "La acción de reparación especificada no existe." });
            }

            var dto = new AccionReparacionResponseDto
            {
                id_accion = accion.IdAccion,
                id_diagnostico = accion.IdDiagnostico,
                descripcion = accion.Descripcion,
                id_estado = accion.IdEstado,
                nombre_estado = accion.IdEstadoNavigation.Nombre,
                id_usuario_autoriza = accion.IdUsuarioAutoriza,
                usuario_autoriza = accion.IdUsuarioAutorizaNavigation != null
                    ? $"{accion.IdUsuarioAutorizaNavigation.Nombre} {accion.IdUsuarioAutorizaNavigation.Apellido}".Trim()
                    : null,
                fecha_autorizacion = accion.FechaAutorizacion,
                orden_ejecucion = accion.OrdenEjecucion,
                costo_mano_obra = accion.CostoManoObra ?? 0,
                fecha_creacion = accion.FechaCreacion,
                fecha_finalizacion = accion.FechaFinalizacion
            };

            return Ok(dto);
        }

        // 3. CREAR
        // POST: api/AccionesReparacion
        [HttpPost]
        public async Task<ActionResult<AccionReparacionResponseDto>> CreateAccionReparacion([FromBody] AccionReparacionCreateDto dto)
        {
            // Validar existencia del diagnóstico
            var diagnosticoExiste = await _context.Diagnosticos.AnyAsync(d => d.IdDiagnostico == dto.id_diagnostico);
            if (!diagnosticoExiste)
            {
                return BadRequest(new { mensaje = "El diagnóstico especificado no existe." });
            }

            // Validar existencia del estado de la acción
            var estadoExiste = await _context.EstadosAccions.AnyAsync(e => e.IdEstado == dto.id_estado);
            if (!estadoExiste)
            {
                return BadRequest(new { mensaje = "El estado de acción especificado no existe." });
            }

            // Validar existencia del usuario autorizador (si aplica)
            if (dto.id_usuario_autoriza.HasValue)
            {
                var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.id_usuario_autoriza.Value);
                if (!usuarioExiste)
                {
                    return BadRequest(new { mensaje = "El usuario especificado para autorización no existe." });
                }
            }

            var accion = new AccionesReparacion
            {
                IdDiagnostico = dto.id_diagnostico,
                Descripcion = dto.descripcion.Trim(),
                IdEstado = dto.id_estado,
                IdUsuarioAutoriza = dto.id_usuario_autoriza,
                FechaAutorizacion = dto.fecha_autorizacion,
                OrdenEjecucion = dto.orden_ejecucion,
                CostoManoObra = dto.costo_mano_obra,
                FechaCreacion = DateTime.Now,
                FechaFinalizacion = dto.fecha_finalizacion
            };

            _context.AccionesReparacions.Add(accion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAccionReparacion),
                new { id = accion.IdAccion },
                new { mensaje = "Acción de reparación registrada correctamente.", id_accion = accion.IdAccion }
            );
        }

        // 4. ACTUALIZAR
        // PUT: api/AccionesReparacion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccionReparacion(int id, [FromBody] AccionReparacionUpdateDto dto)
        {
            var accion = await _context.AccionesReparacions.FindAsync(id);
            if (accion == null)
            {
                return NotFound(new { mensaje = "La acción de reparación a actualizar no existe." });
            }

            // Validar existencia del diagnóstico
            var diagnosticoExiste = await _context.Diagnosticos.AnyAsync(d => d.IdDiagnostico == dto.id_diagnostico);
            if (!diagnosticoExiste)
            {
                return BadRequest(new { mensaje = "El diagnóstico especificado no existe." });
            }

            // Validar existencia del estado de la acción
            var estadoExiste = await _context.EstadosAccions.AnyAsync(e => e.IdEstado == dto.id_estado);
            if (!estadoExiste)
            {
                return BadRequest(new { mensaje = "El estado de acción especificado no existe." });
            }

            // Validar existencia del usuario autorizador (si aplica)
            if (dto.id_usuario_autoriza.HasValue)
            {
                var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.id_usuario_autoriza.Value);
                if (!usuarioExiste)
                {
                    return BadRequest(new { mensaje = "El usuario especificado para autorización no existe." });
                }
            }

            accion.IdDiagnostico = dto.id_diagnostico;
            accion.Descripcion = dto.descripcion.Trim();
            accion.IdEstado = dto.id_estado;
            accion.IdUsuarioAutoriza = dto.id_usuario_autoriza;
            accion.FechaAutorizacion = dto.fecha_autorizacion;
            accion.OrdenEjecucion = dto.orden_ejecucion;
            accion.CostoManoObra = dto.costo_mano_obra;
            accion.FechaFinalizacion = dto.fecha_finalizacion;

            _context.Entry(accion).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Acción de reparación actualizada correctamente." });
        }
    }
}