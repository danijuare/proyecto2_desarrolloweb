namespace proyecto_2_desarrollo_web.DTOs
{
    // Para la creación de un nuevo cliente
    public class ClienteCreateDto
    {
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string? telefono { get; set; }
        public string? email { get; set; }
        public string? direccion { get; set; }
    }

    // Para la actualización de datos de un cliente
    public class ClienteUpdateDto
    {
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string? telefono { get; set; }
        public string? email { get; set; }
        public string? direccion { get; set; }
    }

    // Respuesta JSON que enviamos al cliente
    public class ClienteResponseDto
    {
        public int id_cliente { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string? telefono { get; set; }
        public string? email { get; set; }
        public string? direccion { get; set; }
        public string condicion { get; set; } = "1";
    }
}