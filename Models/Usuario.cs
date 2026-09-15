using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("usuarios")]
[Index("Email", Name = "email", IsUnique = true)]
[Index("IdRol", Name = "id_rol")]
public partial class Usuario
{
    [Key]
    [Column("id_usuario", TypeName = "int(11)")]
    public int IdUsuario { get; set; }

    [Column("id_rol", TypeName = "int(11)")]
    public int IdRol { get; set; }

    [Column("nombre")]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [Column("apellido")]
    [StringLength(80)]
    public string Apellido { get; set; } = null!;

    [Column("email")]
    [StringLength(120)]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Required]
    [Column("activo")]
    public bool? Activo { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    [InverseProperty("IdUsuarioAutorizaNavigation")]
    public virtual ICollection<AccionesReparacion> AccionesReparacions { get; set; } = new List<AccionesReparacion>();

    [ForeignKey("IdRol")]
    [InverseProperty("Usuarios")]
    public virtual Role IdRolNavigation { get; set; } = null!;

    [InverseProperty("IdUsuarioNavigation")]
    public virtual Mecanico? Mecanico { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<TokensSesion> TokensSesions { get; set; } = new List<TokensSesion>();

    [InverseProperty("IdUsuarioAutorizaNavigation")]
    public virtual ICollection<UsoRepuesto> UsoRepuestos { get; set; } = new List<UsoRepuesto>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<UsuariosPermiso> UsuariosPermisos { get; set; } = new List<UsuariosPermiso>();
}
