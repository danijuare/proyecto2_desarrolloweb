using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesTrabajoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdenesTrabajoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODOS
        // GET: api/OrdenesTrabajo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenTrabajoResponseDto>>> GetOrdenesTrabajo()
        {
            var ordenes = await _context.OrdenesTrabajos
                .Include(o => o.IdVehiculoNavigation)
                .Include(o => o.IdMecanicoNavigation)
                    .ThenInclude(m => m!.IdUsuarioNavigation)
                .Include(o => o.IdEstadoNavigation)
                .Select(o => new OrdenTrabajoResponseDto
                {
                    id_orden = o.IdOrden,
                    id_vehiculo = o.IdVehiculo,
                    placa_vehiculo = o.IdVehiculoNavigation.Placa,
                    vehiculo_info = $"{o.IdVehiculoNavigation.Marca} {o.IdVehiculoNavigation.Modelo}",
                    id_mecanico = o.IdMecanico,
                    nombre_mecanico = o.IdMecanicoNavigation != null && o.IdMecanicoNavigation.IdUsuarioNavigation != null
                        ? $"{o.IdMecanicoNavigation.IdUsuarioNavigation.Nombre} {o.IdMecanicoNavigation.IdUsuarioNavigation.Apellido}".Trim()
                        : null,
                    id_estado = o.IdEstado,
                    nombre_estado = o.IdEstadoNavigation.Nombre,
                    kilometraje_ingreso = o.KilometrajeIngreso,
                    fecha_ingreso = o.FechaIngreso,
                    fecha_salida = o.FechaSalida,
                    observaciones = o.Observaciones
                })
                .ToListAsync();

            return Ok(ordenes);
        }

        // 2. OBTENER UNO
        // GET: api/OrdenesTrabajo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenTrabajoResponseDto>> GetOrdenTrabajo(int id)
        {
            var orden = await _context.OrdenesTrabajos
                .Include(o => o.IdVehiculoNavigation)
                .Include(o => o.IdMecanicoNavigation)
                    .ThenInclude(m => m!.IdUsuarioNavigation)
                .Include(o => o.IdEstadoNavigation)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
            {
                return NotFound(new { mensaje = "La orden de trabajo especificada no existe." });
            }

            var dto = new OrdenTrabajoResponseDto
            {
                id_orden = orden.IdOrden,
                id_vehiculo = orden.IdVehiculo,
                placa_vehiculo = orden.IdVehiculoNavigation.Placa,
                vehiculo_info = $"{orden.IdVehiculoNavigation.Marca} {orden.IdVehiculoNavigation.Modelo}",
                id_mecanico = orden.IdMecanico,
                nombre_mecanico = orden.IdMecanicoNavigation != null && orden.IdMecanicoNavigation.IdUsuarioNavigation != null
                    ? $"{orden.IdMecanicoNavigation.IdUsuarioNavigation.Nombre} {orden.IdMecanicoNavigation.IdUsuarioNavigation.Apellido}".Trim()
                    : null,
                id_estado = orden.IdEstado,
                nombre_estado = orden.IdEstadoNavigation.Nombre,
                kilometraje_ingreso = orden.KilometrajeIngreso,
                fecha_ingreso = orden.FechaIngreso,
                fecha_salida = orden.FechaSalida,
                observaciones = orden.Observaciones
            };

            return Ok(dto);
        }

        // 3. CREAR
        // POST: api/OrdenesTrabajo
        [HttpPost]
        public async Task<ActionResult<OrdenTrabajoResponseDto>> CreateOrdenTrabajo([FromBody] OrdenTrabajoCreateDto dto)
        {
            // Validar existencia de vehículo
            var vehiculoExiste = await _context.Vehiculos.AnyAsync(v => v.IdVehiculo == dto.id_vehiculo);
            if (!vehiculoExiste)
            {
                return BadRequest(new { mensaje = "El vehículo especificado no existe." });
            }

            // Validar existencia de mecánico (si viene asignado)
            if (dto.id_mecanico.HasValue)
            {
                var mecanico = await _context.Mecanicos.FindAsync(dto.id_mecanico.Value);
                if (mecanico == null || mecanico.Activo == false)
                {
                    return BadRequest(new { mensaje = "El mecánico especificado no existe o está inactivo." });
                }
            }

            // Validar existencia de estado
            var estadoExiste = await _context.EstadosOrdens.AnyAsync(e => e.IdEstado == dto.id_estado);
            if (!estadoExiste)
            {
                return BadRequest(new { mensaje = "El estado de orden especificado no existe." });
            }

            var orden = new OrdenesTrabajo
            {
                IdVehiculo = dto.id_vehiculo,
                IdMecanico = dto.id_mecanico,
                IdEstado = dto.id_estado,
                KilometrajeIngreso = dto.kilometraje_ingreso,
                FechaIngreso = DateTime.Now,
                FechaSalida = dto.fecha_salida,
                Observaciones = dto.observaciones?.Trim()
            };

            _context.OrdenesTrabajos.Add(orden);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetOrdenTrabajo),
                new { id = orden.IdOrden },
                new { mensaje = "Orden de trabajo registrada correctamente.", id_orden = orden.IdOrden }
            );
        }

        // 4. ACTUALIZAR
        // PUT: api/OrdenesTrabajo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrdenTrabajo(int id, [FromBody] OrdenTrabajoUpdateDto dto)
        {
            var orden = await _context.OrdenesTrabajos.FindAsync(id);
            if (orden == null)
            {
                return NotFound(new { mensaje = "La orden de trabajo a actualizar no existe." });
            }

            // Validar existencia de vehículo
            var vehiculoExiste = await _context.Vehiculos.AnyAsync(v => v.IdVehiculo == dto.id_vehiculo);
            if (!vehiculoExiste)
            {
                return BadRequest(new { mensaje = "El vehículo especificado no existe." });
            }

            // Validar existencia de mecánico (si viene asignado)
            if (dto.id_mecanico.HasValue)
            {
                var mecanico = await _context.Mecanicos.FindAsync(dto.id_mecanico.Value);
                if (mecanico == null || mecanico.Activo == false)
                {
                    return BadRequest(new { mensaje = "El mecánico especificado no existe o está inactivo." });
                }
            }

            // Validar existencia de estado
            var estadoExiste = await _context.EstadosOrdens.AnyAsync(e => e.IdEstado == dto.id_estado);
            if (!estadoExiste)
            {
                return BadRequest(new { mensaje = "El estado de orden especificado no existe." });
            }

            orden.IdVehiculo = dto.id_vehiculo;
            orden.IdMecanico = dto.id_mecanico;
            orden.IdEstado = dto.id_estado;
            orden.KilometrajeIngreso = dto.kilometraje_ingreso;
            orden.FechaSalida = dto.fecha_salida;
            orden.Observaciones = dto.observaciones?.Trim();

            _context.Entry(orden).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Orden de trabajo actualizada correctamente." });
        }
    }
}