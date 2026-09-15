using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("movimientos_inventario")]
[Index("IdUsuario", Name = "id_usuario")]
[Index("IdRepuesto", Name = "idx_movimientos_repuesto")]
public partial class MovimientosInventario
{
    [Key]
    [Column("id_movimiento", TypeName = "int(11)")]
    public int IdMovimiento { get; set; }

    [Column("id_repuesto", TypeName = "int(11)")]
    public int IdRepuesto { get; set; }

    [Column("id_usuario", TypeName = "int(11)")]
    public int IdUsuario { get; set; }

    [Column("tipo")]
    [StringLength(10)]
    public string Tipo { get; set; } = null!;

    [Column("cantidad", TypeName = "int(11)")]
    public int Cantidad { get; set; }

    [Column("motivo")]
    [StringLength(255)]
    public string? Motivo { get; set; }

    [Column("fecha", TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [ForeignKey("IdRepuesto")]
    [InverseProperty("MovimientosInventarios")]
    public virtual Repuesto IdRepuestoNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("MovimientosInventarios")]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
