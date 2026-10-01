namespace proyecto_2_desarrollo_web.DTOs
{
    // Para crear un nuevo usuario
    public class UsuarioCreateDto
    {
        public int id_rol { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }

    // Para actualizar datos de un usuario existente
    public class UsuarioUpdateDto
    {
        public int id_rol { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? password { get; set; } // Opcional por si no se desea cambiar la contraseña
    }

    // Respuesta JSON para el cliente
    public class UsuarioResponseDto
    {
        public int id_usuario { get; set; }
        public int id_rol { get; set; }
        public string nombre_rol { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
    }
}