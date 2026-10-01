namespace proyecto_2_desarrollo_web.DTOs
{
    // Para asignar un permiso a un rol o consultar la relación
    public class RolePermisoDto
    {
        public int id_rol { get; set; }
        public int id_permiso { get; set; }
    }

    // Para asignar múltiples permisos de un solo golpe a un rol
    public class AsignarPermisosRoleDto
    {
        public int id_rol { get; set; }
        public List<int> ids_permisos { get; set; } = new List<int>();
    }

    // Para responder con la lista de permisos que tiene un rol
    public class RoleConPermisosDto
    {
        public int id_rol { get; set; }
        public string nombre_rol { get; set; } = string.Empty;
        public List<PermisoResponseDto> permisos { get; set; } = new List<PermisoResponseDto>();
    }
}