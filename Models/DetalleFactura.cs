using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("detalle_factura")]
[Index("IdFactura", Name = "id_factura")]
public partial class DetalleFactura
{
    [Key]
    [Column("id_detalle", TypeName = "int(11)")]
    public int IdDetalle { get; set; }

    [Column("id_factura", TypeName = "int(11)")]
    public int IdFactura { get; set; }

    [Column("tipo")]
    [StringLength(15)]
    public string Tipo { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(255)]
    public string Descripcion { get; set; } = null!;

    [Column("cantidad", TypeName = "int(11)")]
    public int Cantidad { get; set; }

    [Column("precio_unitario")]
    [Precision(10, 2)]
    public decimal PrecioUnitario { get; set; }

    [Column("subtotal")]
    [Precision(10, 2)]
    public decimal Subtotal { get; set; }

    [ForeignKey("IdFactura")]
    [InverseProperty("DetalleFacturas")]
    public virtual Factura IdFacturaNavigation { get; set; } = null!;
}
