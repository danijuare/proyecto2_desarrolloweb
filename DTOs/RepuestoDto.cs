using System.ComponentModel.DataAnnotations;

namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para respuestas (GET)
    public class RepuestoResponseDto
    {
        public int id_repuesto { get; set; }
        public int? id_categoria { get; set; }
        public string? nombre_categoria { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string? descripcion { get; set; }
        public int stock_actual { get; set; }
        public int stock_minimo { get; set; }
        public decimal precio_unitario { get; set; }
    }

    // DTO para creación y actualización (POST/PUT)
    public class RepuestoCreateUpdateDto
    {
        public int? id_categoria { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(30, ErrorMessage = "El código no puede exceder los 30 caracteres.")]
        public string codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del repuesto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string nombre { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres.")]
        public string? descripcion { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
        public int stock_actual { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int stock_minimo { get; set; } = 0;

        [Range(0.00, 99999999.99, ErrorMessage = "El precio unitario debe ser mayor o igual a 0.")]
        public decimal precio_unitario { get; set; } = 0.00m;
    }
}