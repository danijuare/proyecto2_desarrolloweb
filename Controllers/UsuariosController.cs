using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyecto_2_desarrollo_web.Data;
using proyecto_2_desarrollo_web.DTOs;
using proyecto_2_desarrollo_web.Models;

namespace proyecto_2_desarrollo_web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        // Obtiene únicamente los usuarios activos (activo == true / 1)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Where(u => u.Activo == true)
                .Select(u => new UsuarioResponseDto
                {
                    id_usuario = u.IdUsuario,
                    id_rol = u.IdRol,
                    nombre_rol = u.IdRolNavigation.Nombre,
                    nombre = u.Nombre,
                    apellido = u.Apellido,
                    email = u.Email,
                    activo = u.Activo ?? true,
                    fecha_creacion = u.FechaCreacion
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        // Obtiene un usuario por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.IdUsuario == id && u.Activo == true);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario no existe o está inactivo." });
            }

            var dto = new UsuarioResponseDto
            {
                id_usuario = usuario.IdUsuario,
                id_rol = usuario.IdRol,
                nombre_rol = usuario.IdRolNavigation.Nombre,
                nombre = usuario.Nombre,
                apellido = usuario.Apellido,
                email = usuario.Email,
                activo = usuario.Activo ?? true,
                fecha_creacion = usuario.FechaCreacion
            };

            return Ok(dto);
        }

        // POST: api/Usuarios
        // Registra un nuevo usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> CreateUsuario([FromBody] UsuarioCreateDto dto)
        {
            // Validar que el rol exista
            var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == dto.id_rol);
            if (!rolExiste)
            {
                return BadRequest(new { mensaje = "El rol especificado no existe." });
            }

            // Validar email único
            var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.email);
            if (emailExiste)
            {
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });
            }

            // Crear la entidad
            var usuario = new Usuario
            {
                IdRol = dto.id_rol,
                Nombre = dto.nombre,
                Apellido = dto.apellido,
                Email = dto.email,
                PasswordHash = dto.password, // Recomendado: aplicar hash BCrypt o SHA256 antes de guardar
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, new { mensaje = "Usuario creado correctamente.", id_usuario = usuario.IdUsuario });
        }

        // PUT: api/Usuarios/5
        // Actualiza los datos de un usuario
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null || usuario.Activo == false)
            {
                return NotFound(new { mensaje = "El usuario a actualizar no existe o está inactivo." });
            }

            var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == dto.id_rol);
            if (!rolExiste)
            {
                return BadRequest(new { mensaje = "El rol especificado no existe." });
            }

            // Validar si el nuevo correo ya pertenece a otro usuario
            var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == dto.email && u.IdUsuario != id);
            if (emailExiste)
            {
                return BadRequest(new { mensaje = "El correo electrónico ya está en uso por otro usuario." });
            }

            usuario.IdRol = dto.id_rol;
            usuario.Nombre = dto.nombre;
            usuario.Apellido = dto.apellido;
            usuario.Email = dto.email;

            if (!string.IsNullOrEmpty(dto.password))
            {
                usuario.PasswordHash = dto.password;
            }

            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario actualizado correctamente." });
        }

        // DELETE: api/Usuarios/5
        // Desactivación lógica (pasa el campo activo a false / 0)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "El usuario a desactivar no existe." });
            }

            usuario.Activo = false;
            _context.Entry(usuario).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario desactivado correctamente." });
        }
    }
}