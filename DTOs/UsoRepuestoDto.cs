using System.ComponentModel.DataAnnotations;

namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para respuestas (GET)
    public class UsoRepuestoResponseDto
    {
        public int id_uso { get; set; }
        public int id_accion { get; set; }
        public int id_repuesto { get; set; }
        public string? nombre_repuesto { get; set; }
        public string? codigo_repuesto { get; set; }
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }
        public string justificacion { get; set; } = string.Empty;
        public int id_usuario_autoriza { get; set; }
        public string? nombre_usuario_autoriza { get; set; }
        public DateTime fecha { get; set; }
    }

    // DTO para creación (POST)
    public class UsoRepuestoCreateDto
    {
        [Required(ErrorMessage = "El ID de la acción de reparación es obligatorio.")]
        public int id_accion { get; set; }

        [Required(ErrorMessage = "El ID del repuesto es obligatorio.")]
        public int id_repuesto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int cantidad { get; set; }

        [Range(0.00, 99999999.99, ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal? precio_unitario { get; set; }

        [Required(ErrorMessage = "La justificación es obligatoria.")]
        [StringLength(255, ErrorMessage = "La justificación no puede exceder los 255 caracteres.")]
        public string justificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del usuario que autoriza es obligatorio.")]
        public int id_usuario_autoriza { get; set; }
    }
}