namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para la creación de un diagnóstico
    public class DiagnosticoCreateDto
    {
        public int id_orden { get; set; }
        public int id_mecanico { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public string? gravedad { get; set; } // leve, moderada, grave, critica
    }

    // DTO para la actualización de un diagnóstico
    public class DiagnosticoUpdateDto
    {
        public int id_orden { get; set; }
        public int id_mecanico { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public string? gravedad { get; set; }
    }

    // DTO para la respuesta JSON enviada al cliente
    public class DiagnosticoResponseDto
    {
        public int id_diagnostico { get; set; }
        public int id_orden { get; set; }
        public int id_mecanico { get; set; }
        public string nombre_mecanico { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public string? gravedad { get; set; }
        public DateTime fecha_diagnostico { get; set; }
    }
}