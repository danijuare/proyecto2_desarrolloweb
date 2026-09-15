using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("estados_accion")]
[Index("Nombre", Name = "nombre", IsUnique = true)]
public partial class EstadosAccion
{
    [Key]
    [Column("id_estado", TypeName = "int(11)")]
    public int IdEstado { get; set; }

    [Column("nombre")]
    [StringLength(30)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdEstadoNavigation")]
    public virtual ICollection<AccionesReparacion> AccionesReparacions { get; set; } = new List<AccionesReparacion>();
}
