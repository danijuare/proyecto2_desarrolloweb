using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehiculosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Vehiculos
        // Obtiene todos los vehículos registrados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehiculoResponseDto>>> GetVehiculos()
        {
            var vehiculos = await _context.Vehiculos
                .Include(v => v.IdClienteNavigation)
                .Select(v => new VehiculoResponseDto
                {
                    id_vehiculo = v.IdVehiculo,
                    id_cliente = v.IdCliente,
                    nombre_propietario = $"{v.IdClienteNavigation.Nombre} {v.IdClienteNavigation.Apellido}".Trim(),
                    placa = v.Placa,
                    marca = v.Marca,
                    modelo = v.Modelo,
                    anio = v.Anio,
                    color = v.Color,
                    vin = v.Vin
                })
                .ToListAsync();

            return Ok(vehiculos);
        }

        // GET: api/Vehiculos/5
        // Obtiene un vehículo por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<VehiculoResponseDto>> GetVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos
                .Include(v => v.IdClienteNavigation)
                .FirstOrDefaultAsync(v => v.IdVehiculo == id);

            if (vehiculo == null)
            {
                return NotFound(new { mensaje = "El vehículo especificado no existe." });
            }

            var dto = new VehiculoResponseDto
            {
                id_vehiculo = vehiculo.IdVehiculo,
                id_cliente = vehiculo.IdCliente,
                nombre_propietario = $"{vehiculo.IdClienteNavigation.Nombre} {vehiculo.IdClienteNavigation.Apellido}".Trim(),
                placa = vehiculo.Placa,
                marca = vehiculo.Marca,
                modelo = vehiculo.Modelo,
                anio = vehiculo.Anio,
                color = vehiculo.Color,
                vin = vehiculo.Vin
            };

            return Ok(dto);
        }

        // GET: api/Vehiculos/cliente/5
        // Obtiene todos los vehículos pertenecientes a un cliente específico
        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<VehiculoResponseDto>>> GetVehiculosPorCliente(int idCliente)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.IdCliente == idCliente);
            if (!clienteExiste)
            {
                return NotFound(new { mensaje = "El cliente especificado no existe." });
            }

            var vehiculos = await _context.Vehiculos
                .Include(v => v.IdClienteNavigation)
                .Where(v => v.IdCliente == idCliente)
                .Select(v => new VehiculoResponseDto
                {
                    id_vehiculo = v.IdVehiculo,
                    id_cliente = v.IdCliente,
                    nombre_propietario = $"{v.IdClienteNavigation.Nombre} {v.IdClienteNavigation.Apellido}".Trim(),
                    placa = v.Placa,
                    marca = v.Marca,
                    modelo = v.Modelo,
                    anio = v.Anio,
                    color = v.Color,
                    vin = v.Vin
                })
                .ToListAsync();

            return Ok(vehiculos);
        }

        // POST: api/Vehiculos
        // Registra un nuevo vehículo
        [HttpPost]
        public async Task<ActionResult<VehiculoResponseDto>> CreateVehiculo([FromBody] VehiculoCreateDto dto)
        {
            // Validar que el cliente exista y esté activo
            var cliente = await _context.Clientes.FindAsync(dto.id_cliente);
            if (cliente == null || cliente.Condicion == "0")
            {
                return BadRequest(new { mensaje = "El cliente especificado no existe o está inactivo." });
            }

            // Validar que la placa no esté duplicada
            var placaExiste = await _context.Vehiculos
                .AnyAsync(v => v.Placa.ToLower() == dto.placa.Trim().ToLower());

            if (placaExiste)
            {
                return BadRequest(new { mensaje = "Ya existe un vehículo registrado con esta placa." });
            }

            var vehiculo = new Vehiculo
            {
                IdCliente = dto.id_cliente,
                Placa = dto.placa.Trim().ToUpper(),
                Marca = dto.marca.Trim(),
                Modelo = dto.modelo.Trim(),
                Anio = dto.anio,
                Color = dto.color,
                Vin = dto.vin
            };

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetVehiculo),
                new { id = vehiculo.IdVehiculo },
                new { mensaje = "Vehículo registrado correctamente.", id_vehiculo = vehiculo.IdVehiculo }
            );
        }

        // PUT: api/Vehiculos/5
        // Actualiza los datos de un vehículo
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehiculo(int id, [FromBody] VehiculoUpdateDto dto)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound(new { mensaje = "El vehículo a actualizar no existe." });
            }

            // Validar que el cliente asignado exista
            var cliente = await _context.Clientes.FindAsync(dto.id_cliente);
            if (cliente == null || cliente.Condicion == "0")
            {
                return BadRequest(new { mensaje = "El cliente especificado no existe o está inactivo." });
            }

            // Validar que la placa no pertenezca a otro vehículo
            var placaExiste = await _context.Vehiculos
                .AnyAsync(v => v.Placa.ToLower() == dto.placa.Trim().ToLower() && v.IdVehiculo != id);

            if (placaExiste)
            {
                return BadRequest(new { mensaje = "La placa especificada ya pertenece a otro vehículo." });
            }

            vehiculo.IdCliente = dto.id_cliente;
            vehiculo.Placa = dto.placa.Trim().ToUpper();
            vehiculo.Marca = dto.marca.Trim();
            vehiculo.Modelo = dto.modelo.Trim();
            vehiculo.Anio = dto.anio;
            vehiculo.Color = dto.color;
            vehiculo.Vin = dto.vin;

            _context.Entry(vehiculo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Vehículo actualizado correctamente." });
        }

        // DELETE: api/Vehiculos/5
        // Eliminación física del registro
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound(new { mensaje = "El vehículo a eliminar no existe." });
            }

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Vehículo eliminado correctamente." });
        }
    }
}