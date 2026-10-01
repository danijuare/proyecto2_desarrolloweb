namespace proyecto_2_desarrollo_web.DTOs
{
    // Para crear un nuevo vehículo
    public class VehiculoCreateDto
    {
        public int id_cliente { get; set; }
        public string placa { get; set; } = string.Empty;
        public string marca { get; set; } = string.Empty;
        public string modelo { get; set; } = string.Empty;
        public int? anio { get; set; }
        public string? color { get; set; }
        public string? vin { get; set; }
    }

    // Para actualizar un vehículo existente
    public class VehiculoUpdateDto
    {
        public int id_cliente { get; set; }
        public string placa { get; set; } = string.Empty;
        public string marca { get; set; } = string.Empty;
        public string modelo { get; set; } = string.Empty;
        public int? anio { get; set; }
        public string? color { get; set; }
        public string? vin { get; set; }
    }

    // Respuesta JSON para el cliente con información del propietario
    public class VehiculoResponseDto
    {
        public int id_vehiculo { get; set; }
        public int id_cliente { get; set; }
        public string nombre_propietario { get; set; } = string.Empty;
        public string placa { get; set; } = string.Empty;
        public string marca { get; set; } = string.Empty;
        public string modelo { get; set; } = string.Empty;
        public int? anio { get; set; }
        public string? color { get; set; }
        public string? vin { get; set; }
    }
}