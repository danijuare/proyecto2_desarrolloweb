using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("facturas")]
[Index("IdOrden", Name = "idx_facturas_orden", IsUnique = true)]
public partial class Factura
{
    [Key]
    [Column("id_factura", TypeName = "int(11)")]
    public int IdFactura { get; set; }

    [Column("id_orden", TypeName = "int(11)")]
    public int IdOrden { get; set; }

    [Column("subtotal_mano_obra")]
    [Precision(10, 2)]
    public decimal SubtotalManoObra { get; set; }

    [Column("subtotal_repuestos")]
    [Precision(10, 2)]
    public decimal SubtotalRepuestos { get; set; }

    [Column("impuestos")]
    [Precision(10, 2)]
    public decimal Impuestos { get; set; }

    [Column("total")]
    [Precision(10, 2)]
    public decimal Total { get; set; }

    [Column("estado_pago")]
    [StringLength(20)]
    public string EstadoPago { get; set; } = null!;

    [Column("fecha_emision", TypeName = "datetime")]
    public DateTime FechaEmision { get; set; }

    [InverseProperty("IdFacturaNavigation")]
    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    [ForeignKey("IdOrden")]
    [InverseProperty("Factura")]
    public virtual OrdenesTrabajo IdOrdenNavigation { get; set; } = null!;
}
