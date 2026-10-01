using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasRepuestoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasRepuestoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CategoriasRepuesto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaRepuestoResponseDto>>> GetCategoriasRepuesto()
        {
            var categorias = await _context.CategoriasRepuestos
                .Select(c => new CategoriaRepuestoResponseDto
                {
                    id_categoria = c.IdCategoria,
                    nombre = c.Nombre
                })
                .ToListAsync();

            return Ok(categorias);
        }

        // GET: api/CategoriasRepuesto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaRepuestoResponseDto>> GetCategoriaRepuesto(int id)
        {
            var categoria = await _context.CategoriasRepuestos
                .Where(c => c.IdCategoria == id)
                .Select(c => new CategoriaRepuestoResponseDto
                {
                    id_categoria = c.IdCategoria,
                    nombre = c.Nombre
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound(new { mensaje = "La categoría de repuesto no existe." });
            }

            return Ok(categoria);
        }

        // POST: api/CategoriasRepuesto
        [HttpPost]
        public async Task<ActionResult<CategoriaRepuestoResponseDto>> PostCategoriaRepuesto(CategoriaRepuestoCreateUpdateDto dto)
        {
            // Validar restricción UNIQUE del nombre
            var existeNombre = await _context.CategoriasRepuestos
                .AnyAsync(c => c.Nombre.ToLower() == dto.nombre.Trim().ToLower());

            if (existeNombre)
            {
                return Conflict(new { mensaje = $"Ya existe una categoría con el nombre '{dto.nombre}'." });
            }

            var nuevaCategoria = new CategoriasRepuesto
            {
                Nombre = dto.nombre.Trim()
            };

            _context.CategoriasRepuestos.Add(nuevaCategoria);
            await _context.SaveChangesAsync();

            var responseDto = new CategoriaRepuestoResponseDto
            {
                id_categoria = nuevaCategoria.IdCategoria,
                nombre = nuevaCategoria.Nombre
            };

            return CreatedAtAction(
                nameof(GetCategoriaRepuesto),
                new { id = nuevaCategoria.IdCategoria },
                responseDto
            );
        }

        // PUT: api/CategoriasRepuesto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaRepuesto(int id, CategoriaRepuestoCreateUpdateDto dto)
        {
            var categoria = await _context.CategoriasRepuestos.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new { mensaje = "La categoría a actualizar no existe." });
            }

            // Validar que el nuevo nombre no esté ocupado por otra categoría
            var nombreOcupado = await _context.CategoriasRepuestos
                .AnyAsync(c => c.Nombre.ToLower() == dto.nombre.Trim().ToLower() && c.IdCategoria != id);

            if (nombreOcupado)
            {
                return Conflict(new { mensaje = $"Ya existe otra categoría con el nombre '{dto.nombre}'." });
            }

            categoria.Nombre = dto.nombre.Trim();

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CategoriasRepuesto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaRepuesto(int id)
        {
            var categoria = await _context.CategoriasRepuestos.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new { mensaje = "La categoría no existe." });
            }

            // Verificar si hay repuestos asociados antes de eliminar
            var tieneRepuestos = await _context.Repuestos.AnyAsync(r => r.IdCategoria == id);

            if (tieneRepuestos)
            {
                return BadRequest(new { mensaje = "No se puede eliminar la categoría porque tiene repuestos asociados." });
            }

            _context.CategoriasRepuestos.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}