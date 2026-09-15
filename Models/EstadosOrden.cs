using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("estados_orden")]
[Index("Nombre", Name = "nombre", IsUnique = true)]
public partial class EstadosOrden
{
    [Key]
    [Column("id_estado", TypeName = "int(11)")]
    public int IdEstado { get; set; }

    [Column("nombre")]
    [StringLength(30)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdEstadoNavigation")]
    public virtual ICollection<OrdenesTrabajo> OrdenesTrabajos { get; set; } = new List<OrdenesTrabajo>();
}
