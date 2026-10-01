using System.ComponentModel.DataAnnotations;

namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para respuestas (GET)
    public class FacturaResponseDto
    {
        public int id_factura { get; set; }
        public int id_orden { get; set; }
        public decimal subtotal_mano_obra { get; set; }
        public decimal subtotal_repuestos { get; set; }
        public decimal impuestos { get; set; }
        public decimal total { get; set; }
        public string estado_pago { get; set; } = string.Empty;
        public DateTime fecha_emision { get; set; }
    }

    // DTO para creación y actualización (POST/PUT)
    public class FacturaCreateUpdateDto
    {
        [Required(ErrorMessage = "El ID de la orden de trabajo es obligatorio.")]
        public int id_orden { get; set; }

        [Range(0.00, 99999999.99, ErrorMessage = "El subtotal de mano de obra debe ser mayor o igual a 0.")]
        public decimal subtotal_mano_obra { get; set; } = 0.00m;

        [Range(0.00, 99999999.99, ErrorMessage = "El subtotal de repuestos debe ser mayor o igual a 0.")]
        public decimal subtotal_repuestos { get; set; } = 0.00m;

        [Range(0.00, 99999999.99, ErrorMessage = "Los impuestos deben ser mayor o igual a 0.")]
        public decimal impuestos { get; set; } = 0.00m;

        [Required(ErrorMessage = "El estado de pago es obligatorio.")]
        [RegularExpression("^(pendiente|pagada|anulada)$", ErrorMessage = "El estado de pago debe ser 'pendiente', 'pagada' o 'anulada'.")]
        public string estado_pago { get; set; } = "pendiente";
    }
}