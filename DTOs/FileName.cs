namespace proyecto_2_desarrollo_web.DTOs
{
    // Para crear un nuevo estado de orden
    public class EstadoOrdenCreateDto
    {
        public string nombre { get; set; } = string.Empty;
    }

    // Para actualizar un estado de orden existente
    public class EstadoOrdenUpdateDto
    {
        public string nombre { get; set; } = string.Empty;
    }

    // Respuesta JSON enviada al cliente
    public class EstadoOrdenResponseDto
    {
        public int id_estado { get; set; }
        public string nombre { get; set; } = string.Empty;
    }
}