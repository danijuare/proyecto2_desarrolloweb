namespace proyecto_2_desarrollo_web.DTOs
{
    // Para registrar un nuevo mecánico
    public class MecanicoCreateDto
    {
        public int id_usuario { get; set; }
        public string? especialidad { get; set; }
    }

    // Para actualizar los datos de un mecánico
    public class MecanicoUpdateDto
    {
        public int id_usuario { get; set; }
        public string? especialidad { get; set; }
    }

    // Respuesta JSON enviada al cliente con datos del usuario asociado
    public class MecanicoResponseDto
    {
        public int id_mecanico { get; set; }
        public int id_usuario { get; set; }
        public string nombre_completo { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? especialidad { get; set; }
        public bool activo { get; set; }
    }
}