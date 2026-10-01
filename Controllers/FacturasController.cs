using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FacturasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaResponseDto>>> GetFacturas()
        {
            var facturas = await _context.Facturas
                .OrderByDescending(f => f.FechaEmision)
                .Select(f => new FacturaResponseDto
                {
                    id_factura = f.IdFactura,
                    id_orden = f.IdOrden,
                    subtotal_mano_obra = f.SubtotalManoObra,
                    subtotal_repuestos = f.SubtotalRepuestos,
                    impuestos = f.Impuestos,
                    total = f.Total,
                    estado_pago = f.EstadoPago,
                    fecha_emision = f.FechaEmision
                })
                .ToListAsync();

            return Ok(facturas);
        }

        // GET: api/Facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaResponseDto>> GetFactura(int id)
        {
            var factura = await _context.Facturas
                .Where(f => f.IdFactura == id)
                .Select(f => new FacturaResponseDto
                {
                    id_factura = f.IdFactura,
                    id_orden = f.IdOrden,
                    subtotal_mano_obra = f.SubtotalManoObra,
                    subtotal_repuestos = f.SubtotalRepuestos,
                    impuestos = f.Impuestos,
                    total = f.Total,
                    estado_pago = f.EstadoPago,
                    fecha_emision = f.FechaEmision
                })
                .FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound(new { mensaje = "La factura no existe." });
            }

            return Ok(factura);
        }

        // GET: api/Facturas/orden/5 (Obtener factura por el ID de la orden)
        [HttpGet("orden/{idOrden}")]
        public async Task<ActionResult<FacturaResponseDto>> GetFacturaPorOrden(int idOrden)
        {
            var factura = await _context.Facturas
                .Where(f => f.IdOrden == idOrden)
                .Select(f => new FacturaResponseDto
                {
                    id_factura = f.IdFactura,
                    id_orden = f.IdOrden,
                    subtotal_mano_obra = f.SubtotalManoObra,
                    subtotal_repuestos = f.SubtotalRepuestos,
                    impuestos = f.Impuestos,
                    total = f.Total,
                    estado_pago = f.EstadoPago,
                    fecha_emision = f.FechaEmision
                })
                .FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound(new { mensaje = "No se encontró una factura para la orden de trabajo especificada." });
            }

            return Ok(factura);
        }

        // POST: api/Facturas
        [HttpPost]
        public async Task<ActionResult<FacturaResponseDto>> PostFactura(FacturaCreateUpdateDto dto)
        {
            // Validar que la orden de trabajo exista
            var ordenExiste = await _context.OrdenesTrabajos.AnyAsync(o => o.IdOrden == dto.id_orden);
            if (!ordenExiste)
            {
                return BadRequest(new { mensaje = "La orden de trabajo especificada no existe." });
            }

            // Validar la restricción UNIQUE: una orden solo puede tener una factura
            var yaTieneFactura = await _context.Facturas.AnyAsync(f => f.IdOrden == dto.id_orden);
            if (yaTieneFactura)
            {
                return Conflict(new { mensaje = $"La orden de trabajo {dto.id_orden} ya tiene una factura asociada." });
            }

            var estadoNormalizado = dto.estado_pago.Trim().ToLower();

            // Cálculo automático del total
            decimal totalCalculado = dto.subtotal_mano_obra + dto.subtotal_repuestos + dto.impuestos;

            var nuevaFactura = new Factura
            {
                IdOrden = dto.id_orden,
                SubtotalManoObra = dto.subtotal_mano_obra,
                SubtotalRepuestos = dto.subtotal_repuestos,
                Impuestos = dto.impuestos,
                Total = totalCalculado,
                EstadoPago = estadoNormalizado,
                FechaEmision = DateTime.Now
            };

            _context.Facturas.Add(nuevaFactura);
            await _context.SaveChangesAsync();

            var responseDto = new FacturaResponseDto
            {
                id_factura = nuevaFactura.IdFactura,
                id_orden = nuevaFactura.IdOrden,
                subtotal_mano_obra = nuevaFactura.SubtotalManoObra,
                subtotal_repuestos = nuevaFactura.SubtotalRepuestos,
                impuestos = nuevaFactura.Impuestos,
                total = nuevaFactura.Total,
                estado_pago = nuevaFactura.EstadoPago,
                fecha_emision = nuevaFactura.FechaEmision
            };

            return CreatedAtAction(
                nameof(GetFactura),
                new { id = nuevaFactura.IdFactura },
                responseDto
            );
        }

        // PUT: api/Facturas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, FacturaCreateUpdateDto dto)
        {
            var factura = await _context.Facturas.FindAsync(id);

            if (factura == null)
            {
                return NotFound(new { mensaje = "La factura a actualizar no existe." });
            }

            // Validar que la orden exista si cambia de orden
            if (factura.IdOrden != dto.id_orden)
            {
                var ordenExiste = await _context.OrdenesTrabajos.AnyAsync(o => o.IdOrden == dto.id_orden);
                if (!ordenExiste)
                {
                    return BadRequest(new { mensaje = "La orden de trabajo especificada no existe." });
                }

                var ordenOcupada = await _context.Facturas
                    .AnyAsync(f => f.IdOrden == dto.id_orden && f.IdFactura != id);

                if (ordenOcupada)
                {
                    return Conflict(new { mensaje = $"La orden de trabajo {dto.id_orden} ya está asociada a otra factura." });
                }
            }

            var estadoNormalizado = dto.estado_pago.Trim().ToLower();

            // Recalcular total
            decimal totalCalculado = dto.subtotal_mano_obra + dto.subtotal_repuestos + dto.impuestos;

            factura.IdOrden = dto.id_orden;
            factura.SubtotalManoObra = dto.subtotal_mano_obra;
            factura.SubtotalRepuestos = dto.subtotal_repuestos;
            factura.Impuestos = dto.impuestos;
            factura.Total = totalCalculado;
            factura.EstadoPago = estadoNormalizado;

            _context.Entry(factura).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);

            if (factura == null)
            {
                return NotFound(new { mensaje = "La factura no existe." });
            }

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}