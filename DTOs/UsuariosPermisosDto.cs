using System.ComponentModel.DataAnnotations;

namespace proyecto_2_desarrollo_web.DTOs
{
    // Para asignar o actualizar un permiso específico a un usuario
    public class UsuarioPermisoDto
    {
        public int id_usuario { get; set; }
        public int id_permiso { get; set; }

        [Required(ErrorMessage = "El campo tipo es obligatorio.")]
        [RegularExpression("^(conceder|denegar)$", ErrorMessage = "El tipo debe ser 'conceder' o 'denegar'.")]
        public string tipo { get; set; } = "conceder";
    }

    // Para desasignar un permiso de un usuario
    public class UsuarioPermisoEliminarDto
    {
        public int id_usuario { get; set; }
        public int id_permiso { get; set; }
    }

    // Detalle del permiso asignado directamente
    public class PermisoUsuarioDetalleDto
    {
        public int id_permiso { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string? descripcion { get; set; }
        public string tipo { get; set; } = string.Empty; // 'conceder' o 'denegar'
    }

    // Respuesta JSON estructurada
    public class UsuarioConPermisosDto
    {
        public int id_usuario { get; set; }
        public string nombre_completo { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public List<PermisoUsuarioDetalleDto> permisos_directos { get; set; } = new List<PermisoUsuarioDetalleDto>();
    }
}