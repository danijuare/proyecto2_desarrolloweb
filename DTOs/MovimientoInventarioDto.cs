using System.ComponentModel.DataAnnotations;

namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para respuestas (GET)
    public class MovimientoInventarioResponseDto
    {
        public int id_movimiento { get; set; }
        public int id_repuesto { get; set; }
        public string? nombre_repuesto { get; set; }
        public string? codigo_repuesto { get; set; }
        public int id_usuario { get; set; }
        public string? nombre_usuario { get; set; }
        public string tipo { get; set; } = string.Empty;
        public int cantidad { get; set; }
        public string? motivo { get; set; }
        public DateTime fecha { get; set; }
    }

    // DTO para la creación del movimiento (POST)
    public class MovimientoInventarioCreateDto
    {
        [Required(ErrorMessage = "El ID del repuesto es obligatorio.")]
        public int id_repuesto { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int id_usuario { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
        [RegularExpression("^(entrada|salida)$", ErrorMessage = "El tipo de movimiento debe ser 'entrada' o 'salida'.")]
        public string tipo { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int cantidad { get; set; }

        [StringLength(255, ErrorMessage = "El motivo no puede exceder los 255 caracteres.")]
        public string? motivo { get; set; }
    }
}