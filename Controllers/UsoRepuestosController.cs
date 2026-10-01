using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsoRepuestosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsoRepuestosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UsoRepuestos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsoRepuestoResponseDto>>> GetUsoRepuestos()
        {
            var usos = await _context.UsoRepuestos
                .Include(u => u.IdRepuestoNavigation)
                .Include(u => u.IdUsuarioAutorizaNavigation)
                .OrderByDescending(u => u.Fecha)
                .Select(u => new UsoRepuestoResponseDto
                {
                    id_uso = u.IdUso,
                    id_accion = u.IdAccion,
                    id_repuesto = u.IdRepuesto,
                    nombre_repuesto = u.IdRepuestoNavigation != null ? u.IdRepuestoNavigation.Nombre : null,
                    codigo_repuesto = u.IdRepuestoNavigation != null ? u.IdRepuestoNavigation.Codigo : null,
                    cantidad = u.Cantidad,
                    precio_unitario = u.PrecioUnitario,
                    subtotal = u.Cantidad * u.PrecioUnitario,
                    justificacion = u.Justificacion,
                    id_usuario_autoriza = u.IdUsuarioAutoriza,
                    nombre_usuario_autoriza = u.IdUsuarioAutorizaNavigation != null ? u.IdUsuarioAutorizaNavigation.Nombre : null,
                    fecha = u.Fecha
                })
                .ToListAsync();

            return Ok(usos);
        }

        // GET: api/UsoRepuestos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsoRepuestoResponseDto>> GetUsoRepuesto(int id)
        {
            var uso = await _context.UsoRepuestos
                .Include(u => u.IdRepuestoNavigation)
                .Include(u => u.IdUsuarioAutorizaNavigation)
                .Where(u => u.IdUso == id)
                .Select(u => new UsoRepuestoResponseDto
                {
                    id_uso = u.IdUso,
                    id_accion = u.IdAccion,
                    id_repuesto = u.IdRepuesto,
                    nombre_repuesto = u.IdRepuestoNavigation != null ? u.IdRepuestoNavigation.Nombre : null,
                    codigo_repuesto = u.IdRepuestoNavigation != null ? u.IdRepuestoNavigation.Codigo : null,
                    cantidad = u.Cantidad,
                    precio_unitario = u.PrecioUnitario,
                    subtotal = u.Cantidad * u.PrecioUnitario,
                    justificacion = u.Justificacion,
                    id_usuario_autoriza = u.IdUsuarioAutoriza,
                    nombre_usuario_autoriza = u.IdUsuarioAutorizaNavigation != null ? u.IdUsuarioAutorizaNavigation.Nombre : null,
                    fecha = u.Fecha
                })
                .FirstOrDefaultAsync();

            if (uso == null)
            {
                return NotFound(new { mensaje = "El registro de uso de repuesto no existe." });
            }

            return Ok(uso);
        }

        // GET: api/UsoRepuestos/accion/5 (Obtener todos los repuestos usados en una acción específica)
        [HttpGet("accion/{idAccion}")]
        public async Task<ActionResult<IEnumerable<UsoRepuestoResponseDto>>> GetUsosPorAccion(int idAccion)
        {
            var usos = await _context.UsoRepuestos
                .Include(u => u.IdRepuestoNavigation)
                .Include(u => u.IdUsuarioAutorizaNavigation)
                .Where(u => u.IdAccion == idAccion)
                .Select(u => new UsoRepuestoResponseDto
                {
                    id_uso = u.IdUso,
                    id_accion = u.IdAccion,
                    id_repuesto = u.IdRepuesto,
                    nombre_repuesto = u.IdRepuestoNavigation != null ? u.IdRepuestoNavigation.Nombre : null,
                    codigo_repuesto = u.IdRepuestoNavigation != null ? u.IdRepuestoNavigation.Codigo : null,
                    cantidad = u.Cantidad,
                    precio_unitario = u.PrecioUnitario,
                    subtotal = u.Cantidad * u.PrecioUnitario,
                    justificacion = u.Justificacion,
                    id_usuario_autoriza = u.IdUsuarioAutoriza,
                    nombre_usuario_autoriza = u.IdUsuarioAutorizaNavigation != null ? u.IdUsuarioAutorizaNavigation.Nombre : null,
                    fecha = u.Fecha
                })
                .ToListAsync();

            return Ok(usos);
        }

        // POST: api/UsoRepuestos
        [HttpPost]
        public async Task<ActionResult<UsoRepuestoResponseDto>> PostUsoRepuesto(UsoRepuestoCreateDto dto)
        {
            // Validar que la acción de reparación exista
            var accionExiste = await _context.AccionesReparacions.AnyAsync(a => a.IdAccion == dto.id_accion);
            if (!accionExiste)
            {
                return BadRequest(new { mensaje = "La acción de reparación especificada no existe." });
            }

            // Validar que el usuario que autoriza exista
            var usuarioExiste = await _context.Usuarios.AnyAsync(u => u.IdUsuario == dto.id_usuario_autoriza);
            if (!usuarioExiste)
            {
                return BadRequest(new { mensaje = "El usuario que autoriza no existe." });
            }

            // Validar que el repuesto exista y tenga stock
            var repuesto = await _context.Repuestos.FindAsync(dto.id_repuesto);
            if (repuesto == null)
            {
                return BadRequest(new { mensaje = "El repuesto especificado no existe." });
            }

            if (repuesto.StockActual < dto.cantidad)
            {
                return BadRequest(new { mensaje = $"Stock insuficiente. Stock disponible: {repuesto.StockActual}, solicitado: {dto.cantidad}." });
            }

            // Si el precio unitario no se envía o es 0, tomar el precio actual del catálogo de repuestos
            decimal precioAplicado = (dto.precio_unitario.HasValue && dto.precio_unitario.Value > 0)
                ? dto.precio_unitario.Value
                : repuesto.PrecioUnitario;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Descontar del inventario
                repuesto.StockActual -= dto.cantidad;
                _context.Entry(repuesto).State = EntityState.Modified;

                // 2. Registrar el uso del repuesto
                var nuevoUso = new UsoRepuesto
                {
                    IdAccion = dto.id_accion,
                    IdRepuesto = dto.id_repuesto,
                    Cantidad = dto.cantidad,
                    PrecioUnitario = precioAplicado,
                    Justificacion = dto.justificacion.Trim(),
                    IdUsuarioAutoriza = dto.id_usuario_autoriza,
                    Fecha = DateTime.Now
                };

                _context.UsoRepuestos.Add(nuevoUso);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var responseDto = new UsoRepuestoResponseDto
                {
                    id_uso = nuevoUso.IdUso,
                    id_accion = nuevoUso.IdAccion,
                    id_repuesto = nuevoUso.IdRepuesto,
                    nombre_repuesto = repuesto.Nombre,
                    codigo_repuesto = repuesto.Codigo,
                    cantidad = nuevoUso.Cantidad,
                    precio_unitario = nuevoUso.PrecioUnitario,
                    subtotal = nuevoUso.Cantidad * nuevoUso.PrecioUnitario,
                    justificacion = nuevoUso.Justificacion,
                    id_usuario_autoriza = nuevoUso.IdUsuarioAutoriza,
                    fecha = nuevoUso.Fecha
                };

                return CreatedAtAction(
                    nameof(GetUsoRepuesto),
                    new { id = nuevoUso.IdUso },
                    responseDto
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al registrar el uso del repuesto." });
            }
        }

        // DELETE: api/UsoRepuestos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsoRepuesto(int id)
        {
            var uso = await _context.UsoRepuestos.FindAsync(id);
            if (uso == null)
            {
                return NotFound(new { mensaje = "El registro de uso no existe." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Devolver las unidades al inventario
                var repuesto = await _context.Repuestos.FindAsync(uso.IdRepuesto);
                if (repuesto != null)
                {
                    repuesto.StockActual += uso.Cantidad;
                    _context.Entry(repuesto).State = EntityState.Modified;
                }

                _context.UsoRepuestos.Remove(uso);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al revertir el uso del repuesto." });
            }
        }
    }
}