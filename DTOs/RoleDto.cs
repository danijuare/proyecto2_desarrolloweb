namespace proyecto_2_desarrollo_web.DTOs
{
    public class RoleCreateUpdateDto
    {
        public string nombre { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
    }

    public class RoleResponseDto
    {
        public int id_rol { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
    }
}