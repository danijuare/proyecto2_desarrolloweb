using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("repuestos")]
[Index("Codigo", Name = "codigo", IsUnique = true)]
[Index("IdCategoria", Name = "id_categoria")]
public partial class Repuesto
{
    [Key]
    [Column("id_repuesto", TypeName = "int(11)")]
    public int IdRepuesto { get; set; }

    [Column("id_categoria", TypeName = "int(11)")]
    public int? IdCategoria { get; set; }

    [Column("codigo")]
    [StringLength(30)]
    public string Codigo { get; set; } = null!;

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion")]
    [StringLength(255)]
    public string? Descripcion { get; set; }

    [Column("stock_actual", TypeName = "int(11)")]
    public int StockActual { get; set; }

    [Column("stock_minimo", TypeName = "int(11)")]
    public int StockMinimo { get; set; }

    [Column("precio_unitario")]
    [Precision(10, 2)]
    public decimal PrecioUnitario { get; set; }

    [ForeignKey("IdCategoria")]
    [InverseProperty("Repuestos")]
    public virtual CategoriasRepuesto? IdCategoriaNavigation { get; set; }

    [InverseProperty("IdRepuestoNavigation")]
    public virtual ICollection<MovimientosInventario> MovimientosInventarios { get; set; } = new List<MovimientosInventario>();

    [InverseProperty("IdRepuestoNavigation")]
    public virtual ICollection<UsoRepuesto> UsoRepuestos { get; set; } = new List<UsoRepuesto>();
}
