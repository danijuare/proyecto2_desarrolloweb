namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para la creación de una dependencia
    public class DependenciaAccionCreateDto
    {
        public int id_accion { get; set; }
        public int id_accion_prerequisito { get; set; }
    }

    // DTO para actualizar los requisitos de una acción
    public class DependenciaAccionUpdateDto
    {
        public int id_accion_prerequisito { get; set; }
    }

    // DTO para la respuesta JSON enviada al cliente
    public class DependenciaAccionResponseDto
    {
        public int id_accion { get; set; }
        public string descripcion_accion { get; set; } = string.Empty;
        public int id_accion_prerequisito { get; set; }
        public string descripcion_prerequisito { get; set; } = string.Empty;
    }
}