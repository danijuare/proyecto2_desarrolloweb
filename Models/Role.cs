using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("roles")]
[Index("Nombre", Name = "nombre", IsUnique = true)]
public partial class Role
{
    [Key]
    [Column("id_rol", TypeName = "int(11)")]
    public int IdRol { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(255)]
    public string? Descripcion { get; set; }

    [InverseProperty("IdRolNavigation")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    [ForeignKey("IdRol")]
    [InverseProperty("IdRols")]
    public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();
}
