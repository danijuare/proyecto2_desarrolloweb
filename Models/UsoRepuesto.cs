using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("uso_repuestos")]
[Index("IdRepuesto", Name = "id_repuesto")]
[Index("IdUsuarioAutoriza", Name = "id_usuario_autoriza")]
[Index("IdAccion", Name = "idx_uso_repuestos_accion")]
public partial class UsoRepuesto
{
    [Key]
    [Column("id_uso", TypeName = "int(11)")]
    public int IdUso { get; set; }

    [Column("id_accion", TypeName = "int(11)")]
    public int IdAccion { get; set; }

    [Column("id_repuesto", TypeName = "int(11)")]
    public int IdRepuesto { get; set; }

    [Column("cantidad", TypeName = "int(11)")]
    public int Cantidad { get; set; }

    [Column("precio_unitario")]
    [Precision(10, 2)]
    public decimal PrecioUnitario { get; set; }

    [Column("justificacion")]
    [StringLength(255)]
    public string Justificacion { get; set; } = null!;

    [Column("id_usuario_autoriza", TypeName = "int(11)")]
    public int IdUsuarioAutoriza { get; set; }

    [Column("fecha", TypeName = "datetime")]
    public DateTime Fecha { get; set; }

    [ForeignKey("IdAccion")]
    [InverseProperty("UsoRepuestos")]
    public virtual AccionesReparacion IdAccionNavigation { get; set; } = null!;

    [ForeignKey("IdRepuesto")]
    [InverseProperty("UsoRepuestos")]
    public virtual Repuesto IdRepuestoNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioAutoriza")]
    [InverseProperty("UsoRepuestos")]
    public virtual Usuario IdUsuarioAutorizaNavigation { get; set; } = null!;
}
