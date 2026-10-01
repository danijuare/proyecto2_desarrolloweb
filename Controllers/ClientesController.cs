using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        // Obtiene únicamente los clientes activos (condicion == "1")
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteResponseDto>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .Where(c => c.Condicion == "1")
                .Select(c => new ClienteResponseDto
                {
                    id_cliente = c.IdCliente,
                    nombre = c.Nombre,
                    apellido = c.Apellido,
                    telefono = c.Telefono,
                    email = c.Email,
                    direccion = c.Direccion,
                    condicion = c.Condicion ?? "1"
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // GET: api/Clientes/5
        // Obtiene un cliente activo por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteResponseDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == id && c.Condicion == "1");

            if (cliente == null)
            {
                return NotFound(new { mensaje = "El cliente no existe o está desactivado." });
            }

            var dto = new ClienteResponseDto
            {
                id_cliente = cliente.IdCliente,
                nombre = cliente.Nombre,
                apellido = cliente.Apellido,
                telefono = cliente.Telefono,
                email = cliente.Email,
                direccion = cliente.Direccion,
                condicion = cliente.Condicion ?? "1"
            };

            return Ok(dto);
        }

        // POST: api/Clientes
        // Registra un cliente validando que no se repita Nombre + Apellido
        [HttpPost]
        public async Task<ActionResult<ClienteResponseDto>> CreateCliente([FromBody] ClienteCreateDto dto)
        {
            // Validar si ya existe un cliente activo o inactivo con el mismo nombre y apellido
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.Nombre.ToLower() == dto.nombre.Trim().ToLower()
                            && c.Apellido.ToLower() == dto.apellido.Trim().ToLower());

            if (clienteExiste)
            {
                return BadRequest(new { mensaje = "Ya existe un cliente registrado con ese nombre y apellido." });
            }

            var cliente = new Cliente
            {
                Nombre = dto.nombre.Trim(),
                Apellido = dto.apellido.Trim(),
                Telefono = dto.telefono,
                Email = dto.email,
                Direccion = dto.direccion,
                Condicion = "1" // Activo por defecto
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCliente),
                new { id = cliente.IdCliente },
                new { mensaje = "Cliente registrado correctamente.", id_cliente = cliente.IdCliente }
            );
        }

        // PUT: api/Clientes/5
        // Actualiza los datos verificando que el nuevo Nombre + Apellido no pertenezca a otro cliente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteUpdateDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null || cliente.Condicion == "0")
            {
                return NotFound(new { mensaje = "El cliente a actualizar no existe o está desactivado." });
            }

            // Validar que el nombre y apellido no coincidan con OTRO cliente existente
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.Nombre.ToLower() == dto.nombre.Trim().ToLower()
                            && c.Apellido.ToLower() == dto.apellido.Trim().ToLower()
                            && c.IdCliente != id);

            if (clienteExiste)
            {
                return BadRequest(new { mensaje = "Ya existe otro cliente con el mismo nombre y apellido." });
            }

            cliente.Nombre = dto.nombre.Trim();
            cliente.Apellido = dto.apellido.Trim();
            cliente.Telefono = dto.telefono;
            cliente.Email = dto.email;
            cliente.Direccion = dto.direccion;

            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente actualizado correctamente." });
        }

        // DELETE: api/Clientes/5
        // Desactivación lógica: cambia la propiedad condicion de "1" a "0"
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound(new { mensaje = "El cliente a desactivar no existe." });
            }

            // Asignar "0" para la desactivación lógica
            cliente.Condicion = "0";

            // Marcar el estado como modificado en EF Core
            _context.Entry(cliente).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cliente desactivado correctamente." });
        }
    }
}