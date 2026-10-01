namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para la creación de una orden de trabajo
    public class OrdenTrabajoCreateDto
    {
        public int id_vehiculo { get; set; }
        public int? id_mecanico { get; set; }
        public int id_estado { get; set; }
        public int? kilometraje_ingreso { get; set; }
        public DateTime? fecha_salida { get; set; }
        public string? observaciones { get; set; }
    }

    // DTO para la actualización de una orden de trabajo
    public class OrdenTrabajoUpdateDto
    {
        public int id_vehiculo { get; set; }
        public int? id_mecanico { get; set; }
        public int id_estado { get; set; }
        public int? kilometraje_ingreso { get; set; }
        public DateTime? fecha_salida { get; set; }
        public string? observaciones { get; set; }
    }

    // DTO para la respuesta JSON enviada al cliente
    public class OrdenTrabajoResponseDto
    {
        public int id_orden { get; set; }
        public int id_vehiculo { get; set; }
        public string placa_vehiculo { get; set; } = string.Empty;
        public string vehiculo_info { get; set; } = string.Empty;
        public int? id_mecanico { get; set; }
        public string? nombre_mecanico { get; set; }
        public int id_estado { get; set; }
        public string nombre_estado { get; set; } = string.Empty;
        public int? kilometraje_ingreso { get; set; }
        public DateTime fecha_ingreso { get; set; }
        public DateTime? fecha_salida { get; set; }
        public string? observaciones { get; set; }
    }
}