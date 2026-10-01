using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DependenciasAccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DependenciasAccionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DependenciasAccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DependenciaAccionResponseDto>>> GetDependenciasAccion()
        {
            var dependencias = await _context.DependenciasAccions
                .Include(d => d.IdAccionNavigation)
                .Include(d => d.IdAccionPrerequisitoNavigation)
                .Select(d => new DependenciaAccionResponseDto
                {
                    id_accion = d.IdAccion,
                    descripcion_accion = d.IdAccionNavigation != null ? d.IdAccionNavigation.Descripcion : null,
                    id_accion_prerequisito = d.IdAccionPrerequisito,
                    descripcion_prerequisito = d.IdAccionPrerequisitoNavigation != null ? d.IdAccionPrerequisitoNavigation.Descripcion : null
                })
                .ToListAsync();

            return Ok(dependencias);
        }

        // GET: api/DependenciasAccion/5/3
        [HttpGet("{idAccion}/{idPrerequisito}")]
        public async Task<ActionResult<DependenciaAccionResponseDto>> GetDependenciaAccion(int idAccion, int idPrerequisito)
        {
            var dependencia = await _context.DependenciasAccions
                .Include(d => d.IdAccionNavigation)
                .Include(d => d.IdAccionPrerequisitoNavigation)
                .Where(d => d.IdAccion == idAccion && d.IdAccionPrerequisito == idPrerequisito)
                .Select(d => new DependenciaAccionResponseDto
                {
                    id_accion = d.IdAccion,
                    descripcion_accion = d.IdAccionNavigation != null ? d.IdAccionNavigation.Descripcion : null,
                    id_accion_prerequisito = d.IdAccionPrerequisito,
                    descripcion_prerequisito = d.IdAccionPrerequisitoNavigation != null ? d.IdAccionPrerequisitoNavigation.Descripcion : null
                })
                .FirstOrDefaultAsync();

            if (dependencia == null)
            {
                return NotFound(new { mensaje = "La dependencia especificada no existe." });
            }

            return Ok(dependencia);
        }

        // POST: api/DependenciasAccion
        [HttpPost]
        public async Task<ActionResult<DependenciasAccion>> PostDependenciaAccion(DependenciaAccionCreateDto dto)
        {
            // Validar que la acción y el requisito no sean los mismos (por el CHECK de MySQL)
            if (dto.id_accion == dto.id_accion_prerequisito)
            {
                return BadRequest(new { mensaje = "Una acción no puede ser prerrequisito de sí misma." });
            }

            // Verificar si la combinación ya existe
            var existe = await _context.DependenciasAccions
                .AnyAsync(d => d.IdAccion == dto.id_accion && d.IdAccionPrerequisito == dto.id_accion_prerequisito);

            if (existe)
            {
                return Conflict(new { mensaje = "La relación de dependencia ya existe." });
            }

            // Validar que ambas acciones existan
            var accionExiste = await _context.AccionesReparacions.AnyAsync(a => a.IdAccion == dto.id_accion);
            var prerequisitoExiste = await _context.AccionesReparacions.AnyAsync(a => a.IdAccion == dto.id_accion_prerequisito);

            if (!accionExiste || !prerequisitoExiste)
            {
                return BadRequest(new { mensaje = "Una o ambas acciones de reparación no existen." });
            }

            var nuevaDependencia = new DependenciasAccion
            {
                IdAccion = dto.id_accion,
                IdAccionPrerequisito = dto.id_accion_prerequisito
            };

            _context.DependenciasAccions.Add(nuevaDependencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDependenciaAccion),
                new { idAccion = nuevaDependencia.IdAccion, idPrerequisito = nuevaDependencia.IdAccionPrerequisito },
                dto
            );
        }

        // DELETE: api/DependenciasAccion/5/3
        [HttpDelete("{idAccion}/{idPrerequisito}")]
        public async Task<IActionResult> DeleteDependenciaAccion(int idAccion, int idPrerequisito)
        {
            var dependencia = await _context.DependenciasAccions
                .FindAsync(idAccion, idPrerequisito);

            if (dependencia == null)
            {
                return NotFound(new { mensaje = "La dependencia no existe." });
            }

            _context.DependenciasAccions.Remove(dependencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}