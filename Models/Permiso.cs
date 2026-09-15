using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("permisos")]
[Index("Codigo", Name = "codigo", IsUnique = true)]
public partial class Permiso
{
    [Key]
    [Column("id_permiso", TypeName = "int(11)")]
    public int IdPermiso { get; set; }

    [Column("codigo")]
    [StringLength(60)]
    public string Codigo { get; set; } = null!;

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(255)]
    public string? Descripcion { get; set; }

    [InverseProperty("IdPermisoNavigation")]
    public virtual ICollection<UsuariosPermiso> UsuariosPermisos { get; set; } = new List<UsuariosPermiso>();

    [ForeignKey("IdPermiso")]
    [InverseProperty("IdPermisos")]
    public virtual ICollection<Role> IdRols { get; set; } = new List<Role>();
}
