namespace proyecto_2_desarrollo_web.DTOs
{
    public class PermisoCreateUpdateDto
    {
        public string codigo { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string? descripcion { get; set; }
        public int Condicion { get; set; }
    }

    public class PermisoResponseDto
    {
        public int id_permiso { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string? descripcion { get; set; }
        public int Condicion { get; set; } = 1;
    }
}