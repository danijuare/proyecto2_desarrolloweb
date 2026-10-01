using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DetalleFacturaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DetalleFactura
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleFacturaResponseDto>>> GetDetallesFactura()
        {
            var detalles = await _context.DetalleFacturas
                .Select(d => new DetalleFacturaResponseDto
                {
                    id_detalle = d.IdDetalle,
                    id_factura = d.IdFactura,
                    tipo = d.Tipo,
                    descripcion = d.Descripcion,
                    cantidad = d.Cantidad,
                    precio_unitario = d.PrecioUnitario,
                    subtotal = d.Subtotal
                })
                .ToListAsync();

            return Ok(detalles);
        }

        // GET: api/DetalleFactura/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleFacturaResponseDto>> GetDetalleFactura(int id)
        {
            var detalle = await _context.DetalleFacturas
                .Where(d => d.IdDetalle == id)
                .Select(d => new DetalleFacturaResponseDto
                {
                    id_detalle = d.IdDetalle,
                    id_factura = d.IdFactura,
                    tipo = d.Tipo,
                    descripcion = d.Descripcion,
                    cantidad = d.Cantidad,
                    precio_unitario = d.PrecioUnitario,
                    subtotal = d.Subtotal
                })
                .FirstOrDefaultAsync();

            if (detalle == null)
            {
                return NotFound(new { mensaje = "El detalle de la factura no existe." });
            }

            return Ok(detalle);
        }

        // GET: api/DetalleFactura/factura/5 (Obtener todos los ítems de una factura)
        [HttpGet("factura/{idFactura}")]
        public async Task<ActionResult<IEnumerable<DetalleFacturaResponseDto>>> GetDetallesPorFactura(int idFactura)
        {
            var detalles = await _context.DetalleFacturas
                .Where(d => d.IdFactura == idFactura)
                .Select(d => new DetalleFacturaResponseDto
                {
                    id_detalle = d.IdDetalle,
                    id_factura = d.IdFactura,
                    tipo = d.Tipo,
                    descripcion = d.Descripcion,
                    cantidad = d.Cantidad,
                    precio_unitario = d.PrecioUnitario,
                    subtotal = d.Subtotal
                })
                .ToListAsync();

            return Ok(detalles);
        }

        // POST: api/DetalleFactura
        [HttpPost]
        public async Task<ActionResult<DetalleFacturaResponseDto>> PostDetalleFactura(DetalleFacturaCreateUpdateDto dto)
        {
            var factura = await _context.Facturas.FindAsync(dto.id_factura);
            if (factura == null)
            {
                return BadRequest(new { mensaje = "La factura especificada no existe." });
            }

            var tipoNormalizado = dto.tipo.Trim().ToLower();
            decimal subtotalCalculado = dto.cantidad * dto.precio_unitario;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Guardar ítem de detalle
                var nuevoDetalle = new DetalleFactura
                {
                    IdFactura = dto.id_factura,
                    Tipo = tipoNormalizado,
                    Descripcion = dto.descripcion.Trim(),
                    Cantidad = dto.cantidad,
                    PrecioUnitario = dto.precio_unitario,
                    Subtotal = subtotalCalculado
                };

                _context.DetalleFacturas.Add(nuevoDetalle);
                await _context.SaveChangesAsync();

                // 2. Recalcular los totales de la cabecera de la factura
                await RecalcularTotalesFactura(dto.id_factura);

                await transaction.CommitAsync();

                var responseDto = new DetalleFacturaResponseDto
                {
                    id_detalle = nuevoDetalle.IdDetalle,
                    id_factura = nuevoDetalle.IdFactura,
                    tipo = nuevoDetalle.Tipo,
                    descripcion = nuevoDetalle.Descripcion,
                    cantidad = nuevoDetalle.Cantidad,
                    precio_unitario = nuevoDetalle.PrecioUnitario,
                    subtotal = nuevoDetalle.Subtotal
                };

                return CreatedAtAction(
                    nameof(GetDetalleFactura),
                    new { id = nuevoDetalle.IdDetalle },
                    responseDto
                );
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al agregar el detalle de la factura." });
            }
        }

        // PUT: api/DetalleFactura/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetalleFactura(int id, DetalleFacturaCreateUpdateDto dto)
        {
            var detalle = await _context.DetalleFacturas.FindAsync(id);
            if (detalle == null)
            {
                return NotFound(new { mensaje = "El detalle a actualizar no existe." });
            }

            var factura = await _context.Facturas.FindAsync(dto.id_factura);
            if (factura == null)
            {
                return BadRequest(new { mensaje = "La factura especificada no existe." });
            }

            var tipoNormalizado = dto.tipo.Trim().ToLower();
            int idFacturaAnterior = detalle.IdFactura;
            decimal subtotalCalculado = dto.cantidad * dto.precio_unitario;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Actualizar campos
                detalle.IdFactura = dto.id_factura;
                detalle.Tipo = tipoNormalizado;
                detalle.Descripcion = dto.descripcion.Trim();
                detalle.Cantidad = dto.cantidad;
                detalle.PrecioUnitario = dto.precio_unitario;
                detalle.Subtotal = subtotalCalculado;

                _context.Entry(detalle).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Recalcular la factura actual
                await RecalcularTotalesFactura(dto.id_factura);

                // Si se cambió de factura, recalcular también la factura previa
                if (idFacturaAnterior != dto.id_factura)
                {
                    await RecalcularTotalesFactura(idFacturaAnterior);
                }

                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar el detalle de la factura." });
            }
        }

        // DELETE: api/DetalleFactura/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalleFactura(int id)
        {
            var detalle = await _context.DetalleFacturas.FindAsync(id);
            if (detalle == null)
            {
                return NotFound(new { mensaje = "El detalle no existe." });
            }

            int idFactura = detalle.IdFactura;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.DetalleFacturas.Remove(detalle);
                await _context.SaveChangesAsync();

                // Recalcular los totales de la factura tras la eliminación
                await RecalcularTotalesFactura(idFactura);

                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Ocurrió un error al eliminar el detalle de la factura." });
            }
        }

        // Método auxiliar para actualizar los subtotales y total de la factura
        private async Task RecalcularTotalesFactura(int idFactura)
        {
            var factura = await _context.Facturas.FindAsync(idFactura);
            if (factura != null)
            {
                var subtotales = await _context.DetalleFacturas
                    .Where(d => d.IdFactura == idFactura)
                    .GroupBy(d => d.Tipo)
                    .Select(g => new
                    {
                        Tipo = g.Key,
                        Subtotal = g.Sum(d => d.Subtotal)
                    })
                    .ToListAsync();

                factura.SubtotalManoObra = subtotales.FirstOrDefault(s => s.Tipo == "mano_obra")?.Subtotal ?? 0.00m;
                factura.SubtotalRepuestos = subtotales.FirstOrDefault(s => s.Tipo == "repuesto")?.Subtotal ?? 0.00m;
                factura.Total = factura.SubtotalManoObra + factura.SubtotalRepuestos + factura.Impuestos;

                _context.Entry(factura).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}