namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para crear un nuevo estado de acción
    public class EstadoAccionCreateDto
    {
        public string nombre { get; set; } = string.Empty;
    }

    // DTO para actualizar un estado de acción existente
    public class EstadoAccionUpdateDto
    {
        public string nombre { get; set; } = string.Empty;
    }

    // DTO para la respuesta JSON enviada al cliente
    public class EstadoAccionResponseDto
    {
        public int id_estado { get; set; }
        public string nombre { get; set; } = string.Empty;
    }
}