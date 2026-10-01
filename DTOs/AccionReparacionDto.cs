namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para la creación de una acción de reparación
    public class AccionReparacionCreateDto
    {
        public int id_diagnostico { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public int id_estado { get; set; }
        public int? id_usuario_autoriza { get; set; }
        public DateTime? fecha_autorizacion { get; set; }
        public int orden_ejecucion { get; set; } = 1;
        public decimal costo_mano_obra { get; set; } = 0;
        public DateTime? fecha_finalizacion { get; set; }
    }

    // DTO para la actualización de una acción de reparación
    public class AccionReparacionUpdateDto
    {
        public int id_diagnostico { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public int id_estado { get; set; }
        public int? id_usuario_autoriza { get; set; }
        public DateTime? fecha_autorizacion { get; set; }
        public int orden_ejecucion { get; set; } = 1;
        public decimal costo_mano_obra { get; set; } = 0;
        public DateTime? fecha_finalizacion { get; set; }
    }

    // DTO para la respuesta JSON enviada al cliente
    public class AccionReparacionResponseDto
    {
        public int id_accion { get; set; }
        public int id_diagnostico { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public int id_estado { get; set; }
        public string nombre_estado { get; set; } = string.Empty;
        public int? id_usuario_autoriza { get; set; }
        public string? usuario_autoriza { get; set; }
        public DateTime? fecha_autorizacion { get; set; }
        public int orden_ejecucion { get; set; }
        public decimal costo_mano_obra { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateTime? fecha_finalizacion { get; set; }
    }
}