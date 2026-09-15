using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("clientes")]
public partial class Cliente
{
    [Key]
    [Column("id_cliente", TypeName = "int(11)")]
    public int IdCliente { get; set; }

    [Column("nombre")]
    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [Column("apellido")]
    [StringLength(80)]
    public string Apellido { get; set; } = null!;

    [Column("telefono")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [Column("email")]
    [StringLength(120)]
    public string? Email { get; set; }

    [Column("direccion")]
    [StringLength(255)]
    public string? Direccion { get; set; }

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
