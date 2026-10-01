using System.ComponentModel.DataAnnotations;

namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para respuestas (GET)
    public class DetalleFacturaResponseDto
    {
        public int id_detalle { get; set; }
        public int id_factura { get; set; }
        public string tipo { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public int cantidad { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }
    }

    // DTO para creación y actualización (POST/PUT)
    public class DetalleFacturaCreateUpdateDto
    {
        [Required(ErrorMessage = "El ID de la factura es obligatorio.")]
        public int id_factura { get; set; }

        [Required(ErrorMessage = "El tipo de detalle es obligatorio.")]
        [RegularExpression("^(mano_obra|repuesto)$", ErrorMessage = "El tipo debe ser 'mano_obra' o 'repuesto'.")]
        public string tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(255, ErrorMessage = "La descripción no puede exceder los 255 caracteres.")]
        public string descripcion { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int cantidad { get; set; } = 1;

        [Range(0.00, 99999999.99, ErrorMessage = "El precio unitario debe ser mayor o igual a 0.")]
        public decimal precio_unitario { get; set; }
    }
}