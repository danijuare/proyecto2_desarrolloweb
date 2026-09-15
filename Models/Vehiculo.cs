using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("vehiculos")]
[Index("IdCliente", Name = "idx_vehiculos_cliente")]
[Index("Placa", Name = "placa", IsUnique = true)]
public partial class Vehiculo
{
    [Key]
    [Column("id_vehiculo", TypeName = "int(11)")]
    public int IdVehiculo { get; set; }

    [Column("id_cliente", TypeName = "int(11)")]
    public int IdCliente { get; set; }

    [Column("placa")]
    [StringLength(15)]
    public string Placa { get; set; } = null!;

    [Column("marca")]
    [StringLength(50)]
    public string Marca { get; set; } = null!;

    [Column("modelo")]
    [StringLength(50)]
    public string Modelo { get; set; } = null!;

    [Column("anio", TypeName = "int(11)")]
    public int? Anio { get; set; }

    [Column("color")]
    [StringLength(30)]
    public string? Color { get; set; }

    [Column("vin")]
    [StringLength(50)]
    public string? Vin { get; set; }

    [ForeignKey("IdCliente")]
    [InverseProperty("Vehiculos")]
    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    [InverseProperty("IdVehiculoNavigation")]
    public virtual ICollection<OrdenesTrabajo> OrdenesTrabajos { get; set; } = new List<OrdenesTrabajo>();
}
