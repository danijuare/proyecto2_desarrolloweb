using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepuestosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RepuestosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Repuestos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepuestoResponseDto>>> GetRepuestos()
        {
            var repuestos = await _context.Repuestos
                .Include(r => r.IdCategoriaNavigation)
                .Select(r => new RepuestoResponseDto
                {
                    id_repuesto = r.IdRepuesto,
                    id_categoria = r.IdCategoria,
                    nombre_categoria = r.IdCategoriaNavigation != null ? r.IdCategoriaNavigation.Nombre : null,
                    codigo = r.Codigo,
                    nombre = r.Nombre,
                    descripcion = r.Descripcion,
                    stock_actual = r.StockActual,
                    stock_minimo = r.StockMinimo,
                    precio_unitario = r.PrecioUnitario
                })
                .ToListAsync();

            return Ok(repuestos);
        }

        // GET: api/Repuestos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RepuestoResponseDto>> GetRepuesto(int id)
        {
            var repuesto = await _context.Repuestos
                .Include(r => r.IdCategoriaNavigation)
                .Where(r => r.IdRepuesto == id)
                .Select(r => new RepuestoResponseDto
                {
                    id_repuesto = r.IdRepuesto,
                    id_categoria = r.IdCategoria,
                    nombre_categoria = r.IdCategoriaNavigation != null ? r.IdCategoriaNavigation.Nombre : null,
                    codigo = r.Codigo,
                    nombre = r.Nombre,
                    descripcion = r.Descripcion,
                    stock_actual = r.StockActual,
                    stock_minimo = r.StockMinimo,
                    precio_unitario = r.PrecioUnitario
                })
                .FirstOrDefaultAsync();

            if (repuesto == null)
            {
                return NotFound(new { mensaje = "El repuesto no existe." });
            }

            return Ok(repuesto);
        }

        // POST: api/Repuestos
        [HttpPost]
        public async Task<ActionResult<RepuestoResponseDto>> PostRepuesto(RepuestoCreateUpdateDto dto)
        {
            // Validar si el código ya existe (restricción UNIQUE)
            var existeCodigo = await _context.Repuestos
                .AnyAsync(r => r.Codigo.ToLower() == dto.codigo.Trim().ToLower());

            if (existeCodigo)
            {
                return Conflict(new { mensaje = $"Ya existe un repuesto con el código '{dto.codigo}'." });
            }

            // Validar que la categoría exista si se proporcionó un id_categoria
            if (dto.id_categoria.HasValue)
            {
                var categoriaExiste = await _context.CategoriasRepuestos
                    .AnyAsync(c => c.IdCategoria == dto.id_categoria.Value);

                if (!categoriaExiste)
                {
                    return BadRequest(new { mensaje = "La categoría especificada no existe." });
                }
            }

            var nuevoRepuesto = new Repuesto
            {
                IdCategoria = dto.id_categoria,
                Codigo = dto.codigo.Trim(),
                Nombre = dto.nombre.Trim(),
                Descripcion = dto.descripcion?.Trim(),
                StockActual = dto.stock_actual,
                StockMinimo = dto.stock_minimo,
                PrecioUnitario = dto.precio_unitario
            };

            _context.Repuestos.Add(nuevoRepuesto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRepuesto),
                new { id = nuevoRepuesto.IdRepuesto },
                dto
            );
        }

        // PUT: api/Repuestos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepuesto(int id, RepuestoCreateUpdateDto dto)
        {
            var repuesto = await _context.Repuestos.FindAsync(id);

            if (repuesto == null)
            {
                return NotFound(new { mensaje = "El repuesto a actualizar no existe." });
            }

            // Validar código duplicado en otro registro
            var codigoOcupado = await _context.Repuestos
                .AnyAsync(r => r.Codigo.ToLower() == dto.codigo.Trim().ToLower() && r.IdRepuesto != id);

            if (codigoOcupado)
            {
                return Conflict(new { mensaje = $"El código '{dto.codigo}' ya pertenece a otro repuesto." });
            }

            // Validar categoría si no es nula
            if (dto.id_categoria.HasValue)
            {
                var categoriaExiste = await _context.CategoriasRepuestos
                    .AnyAsync(c => c.IdCategoria == dto.id_categoria.Value);

                if (!categoriaExiste)
                {
                    return BadRequest(new { mensaje = "La categoría especificada no existe." });
                }
            }

            repuesto.IdCategoria = dto.id_categoria;
            repuesto.Codigo = dto.codigo.Trim();
            repuesto.Nombre = dto.nombre.Trim();
            repuesto.Descripcion = dto.descripcion?.Trim();
            repuesto.StockActual = dto.stock_actual;
            repuesto.StockMinimo = dto.stock_minimo;
            repuesto.PrecioUnitario = dto.precio_unitario;

            _context.Entry(repuesto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Repuestos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRepuesto(int id)
        {
            var repuesto = await _context.Repuestos.FindAsync(id);

            if (repuesto == null)
            {
                return NotFound(new { mensaje = "El repuesto no existe." });
            }

            // Validar si el repuesto tiene movimientos o usos registrados antes de borrar
            var tieneUsos = await _context.UsoRepuestos.AnyAsync(u => u.IdRepuesto == id);
            var tieneMovimientos = await _context.MovimientosInventarios.AnyAsync(m => m.IdRepuesto == id);

            if (tieneUsos || tieneMovimientos)
            {
                return BadRequest(new { mensaje = "No se puede eliminar el repuesto porque tiene registros de inventario o uso asociados." });
            }

            _context.Repuestos.Remove(repuesto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}