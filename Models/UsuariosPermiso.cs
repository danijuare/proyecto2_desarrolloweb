using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[PrimaryKey("IdUsuario", "IdPermiso")]
[Table("usuarios_permisos")]
[Index("IdPermiso", Name = "id_permiso")]
public partial class UsuariosPermiso
{
    [Key]
    [Column("id_usuario", TypeName = "int(11)")]
    public int IdUsuario { get; set; }

    [Key]
    [Column("id_permiso", TypeName = "int(11)")]
    public int IdPermiso { get; set; }

    [Column("tipo")]
    [StringLength(10)]
    public string Tipo { get; set; } = null!;

    [ForeignKey("IdPermiso")]
    [InverseProperty("UsuariosPermisos")]
    public virtual Permiso IdPermisoNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("UsuariosPermisos")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
