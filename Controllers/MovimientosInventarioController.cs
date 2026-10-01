using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovimientosInventarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MovimientosInventario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoInventarioResponseDto>>> GetMovimientos()
        {
            var movimientos = await _context.MovimientosInventarios
                .Include(m => m.IdRepuestoNavigation)
                .Include(m => m.IdUsuarioNavigation)
                .OrderByDescending(m => m.Fecha)
                .Select(m => new MovimientoInventarioResponseDto
                {
                    id_movimiento = m.IdMovimiento,
                    id_repuesto = m.IdRepuesto,
                    nombre_repuesto = m.IdRepuestoNavigation != null ? m.IdRepuestoNavigation.Nombre : null,
                    codigo_repuesto = m.IdRepuestoNavigation != null ? m.IdRepuestoNavigation.Codigo : null,
                    id_usuario = m.IdUsuario,
                    // Si tu propiedad en Usuario.cs es Nombre, usa m.IdUsuarioNavigation.Nombre
                    nombre_usuario = m.IdUsuarioNavigation != null ? m.IdUsuarioNavigation.Nombre : null,
                    tipo = m.Tipo,
                    cantidad = m.Cantidad,
                    motivo = m.Motivo,
                    fecha = m.Fecha
                })
                .ToListAsync();

            return Ok(movimientos);
        }

        // GET: api/MovimientosInventario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoInventarioResponseDto>> GetMovimiento(int id)
        {
            var movimiento = await _context.MovimientosInventarios
                .Include(m => m.IdRepuestoNavigation)
                .Include(m => m.IdUsuarioNavigation)
                .Where(m => m.IdMovimiento == id)
                .Select(m => new MovimientoInventarioResponseDto
                {
                    id_movimiento = m.IdMovimiento,
                    id_repuesto = m.IdRepuesto,
                    nombre_repuesto = m.IdRepuestoNavigation != null ? m.IdRepuestoNavigation.Nombre : null,
                    codigo_repuesto = m.IdRepuestoNavigation != null ? m.IdRepuestoNavigation.Codigo : null,
                    id_usuario = m.IdUsuario,
                    // Si tu propiedad en Usuario.cs es Nombre, usa m.IdUsuarioNavigation.Nombre
                    nombre_usuario = m.IdUsuarioNavigation != null ? m.IdUsuarioNavigation.Nombre : null,
                    tipo = m.Tipo,
                    cantidad = m.Cantidad,
                    motivo = m.Motivo,
                    fecha = m.Fecha
                })
                .FirstOrDefaultAsync();

            if (movimiento == null)
            {
                return NotFound(new { mensaje = "El movimiento de inventario no existe." });
            }

            return Ok(movimiento);
        }

        // POST: api/MovimientosInventario
        [HttpPost]
        public async Task<ActionResult<MovimientoInventarioResponseDto>> PostMovimiento(MovimientoInventarioCreateDto dto)
        {
            var repuesto = await _context.Repuestos.FindAsync(dto.id_repuesto);
            if (repuesto == null)
            {
                return BadRequest(new { mensaje = "El repuesto especificado no existe." });
            }

            var usuario = await _context.Usuarios.FindAsync(dto.id_usuario);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El usuario especificado no existe." });
            }

            var tipoNormalizado = dto.tipo.Trim().ToLower();

            if (tipoNormalizado == "salida" && repuesto.StockActual < dto.cantidad)
            {
                return BadRequest(new { mensaje = $"Stock insuficiente. Stock actual: {repuesto.StockActual}, solicitado: {dto.cantidad}." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (tipoNormalizado == "entrada")
                {
                    repuesto.StockActual += dto.cantidad;
                }
                else if (tipoNormalizado == "salida")
                {
                    repuesto.StockActual -= dto.cantidad;
                }

                _context.Entry(repuesto).State = EntityState.Modified;

                var nuevoMovimiento = new MovimientosInventario
                {
                    IdRepuesto = dto.id_repuesto,
                    IdUsuario = dto.id_usuario,
                    Tipo = tipoNormalizado,
                    Cantidad = dto.cantidad,
                    Motivo = dto.motivo?.Trim(),
                    Fecha = DateTime.Now
                };

                _context.MovimientosInventarios.Add(nuevoMovimiento);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var responseDto = new MovimientoInventarioResponseDto
                {
                    id_movimiento = nuevoMovimiento.IdMovimiento,
                    id_repuesto = nuevoMovimiento.IdRepuesto,
                    nombre_repuesto = repuesto.Nombre,
                    codigo_repuesto = repuesto.Codigo,
                    id_usuario = nuevoMovimiento.IdUsuario,
                    nombre_usuario = usuario.Nombre, // Ajustar según el nombre exacto de la propiedad en Usuario.cs
                    tipo = nuevoMovimiento.Tipo,
                    cantidad = nuevoMovimiento.Cantidad,
                    motivo = nuevoMovimiento.Motivo,
                    fecha = nuevoMovimiento.Fecha
                };

                return CreatedAtAction(
                    nameof(GetMovimiento),
                    new { id = nuevoMovimiento.IdMovimiento },
                    responseDto
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al procesar el movimiento de inventario." });
            }
        }
    }
}