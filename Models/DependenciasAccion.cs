namespace proyecto_2_desarrollo_web.Models
{
    public class DependenciasAccion
    {
        public int IdAccion { get; set; }
        public virtual AccionesReparacion? IdAccionNavigation { get; set; }

        public int IdAccionPrerequisito { get; set; }
        public virtual AccionesReparacion? IdAccionPrerequisitoNavigation { get; set; }
    }
}