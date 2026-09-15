using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("ordenes_trabajo")]
[Index("IdEstado", Name = "id_estado")]
[Index("IdMecanico", Name = "idx_ordenes_mecanico")]
[Index("IdVehiculo", Name = "idx_ordenes_vehiculo")]
public partial class OrdenesTrabajo
{
    [Key]
    [Column("id_orden", TypeName = "int(11)")]
    public int IdOrden { get; set; }

    [Column("id_vehiculo", TypeName = "int(11)")]
    public int IdVehiculo { get; set; }

    [Column("id_mecanico", TypeName = "int(11)")]
    public int? IdMecanico { get; set; }

    [Column("id_estado", TypeName = "int(11)")]
    public int IdEstado { get; set; }

    [Column("kilometraje_ingreso", TypeName = "int(11)")]
    public int? KilometrajeIngreso { get; set; }

    [Column("fecha_ingreso", TypeName = "datetime")]
    public DateTime FechaIngreso { get; set; }

    [Column("fecha_salida", TypeName = "datetime")]
    public DateTime? FechaSalida { get; set; }

    [Column("observaciones")]
    [StringLength(500)]
    public string? Observaciones { get; set; }

    [InverseProperty("IdOrdenNavigation")]
    public virtual ICollection<Diagnostico> Diagnosticos { get; set; } = new List<Diagnostico>();

    [InverseProperty("IdOrdenNavigation")]
    public virtual Factura? Factura { get; set; }

    [ForeignKey("IdEstado")]
    [InverseProperty("OrdenesTrabajos")]
    public virtual EstadosOrden IdEstadoNavigation { get; set; } = null!;

    [ForeignKey("IdMecanico")]
    [InverseProperty("OrdenesTrabajos")]
    public virtual Mecanico? IdMecanicoNavigation { get; set; }

    [ForeignKey("IdVehiculo")]
    [InverseProperty("OrdenesTrabajos")]
    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
