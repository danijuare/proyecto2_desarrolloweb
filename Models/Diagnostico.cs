using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("diagnosticos")]
[Index("IdMecanico", Name = "id_mecanico")]
[Index("IdOrden", Name = "idx_diagnosticos_orden")]
public partial class Diagnostico
{
    [Key]
    [Column("id_diagnostico", TypeName = "int(11)")]
    public int IdDiagnostico { get; set; }

    [Column("id_orden", TypeName = "int(11)")]
    public int IdOrden { get; set; }

    [Column("id_mecanico", TypeName = "int(11)")]
    public int IdMecanico { get; set; }

    [Column("descripcion")]
    [StringLength(500)]
    public string Descripcion { get; set; } = null!;

    [Column("gravedad")]
    [StringLength(20)]
    public string? Gravedad { get; set; }

    [Column("fecha_diagnostico", TypeName = "datetime")]
    public DateTime FechaDiagnostico { get; set; }

    [InverseProperty("IdDiagnosticoNavigation")]
    public virtual ICollection<AccionesReparacion> AccionesReparacions { get; set; } = new List<AccionesReparacion>();

    [ForeignKey("IdMecanico")]
    [InverseProperty("Diagnosticos")]
    public virtual Mecanico IdMecanicoNavigation { get; set; } = null!;

    [ForeignKey("IdOrden")]
    [InverseProperty("Diagnosticos")]
    public virtual OrdenesTrabajo IdOrdenNavigation { get; set; } = null!;
}
