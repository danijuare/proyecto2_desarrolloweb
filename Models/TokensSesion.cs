using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("tokens_sesion")]
[Index("IdUsuario", Name = "id_usuario")]
public partial class TokensSesion
{
    [Key]
    [Column("id_token", TypeName = "int(11)")]
    public int IdToken { get; set; }

    [Column("id_usuario", TypeName = "int(11)")]
    public int IdUsuario { get; set; }

    [Column("token")]
    [StringLength(500)]
    public string Token { get; set; } = null!;

    [Column("fecha_emision", TypeName = "datetime")]
    public DateTime FechaEmision { get; set; }

    [Column("fecha_expiracion", TypeName = "datetime")]
    public DateTime FechaExpiracion { get; set; }

    [Column("revocado")]
    public bool Revocado { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("TokensSesions")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
