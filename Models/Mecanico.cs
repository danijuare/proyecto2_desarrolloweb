using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("mecanicos")]
[Index("IdUsuario", Name = "id_usuario", IsUnique = true)]
public partial class Mecanico
{
    [Key]
    [Column("id_mecanico", TypeName = "int(11)")]
    public int IdMecanico { get; set; }

    [Column("id_usuario", TypeName = "int(11)")]
    public int IdUsuario { get; set; }

    [Column("especialidad")]
    [StringLength(100)]
    public string? Especialidad { get; set; }

    [Required]
    [Column("activo")]
    public bool? Activo { get; set; }

    [InverseProperty("IdMecanicoNavigation")]
    public virtual ICollection<Diagnostico> Diagnosticos { get; set; } = new List<Diagnostico>();

    [ForeignKey("IdUsuario")]
    [InverseProperty("Mecanico")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    [InverseProperty("IdMecanicoNavigation")]
    public virtual ICollection<OrdenesTrabajo> OrdenesTrabajos { get; set; } = new List<OrdenesTrabajo>();
}
