using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("acciones_reparacion")]
[Index("IdEstado", Name = "id_estado")]
[Index("IdUsuarioAutoriza", Name = "id_usuario_autoriza")]
[Index("IdDiagnostico", Name = "idx_acciones_diagnostico")]
public partial class AccionesReparacion
{
    [Key]
    [Column("id_accion", TypeName = "int(11)")]
    public int IdAccion { get; set; }

    [Column("id_diagnostico", TypeName = "int(11)")]
    public int IdDiagnostico { get; set; }

    [Column("descripcion")]
    [StringLength(500)]
    public string Descripcion { get; set; } = null!;

    [Column("id_estado", TypeName = "int(11)")]
    public int IdEstado { get; set; }

    [Column("id_usuario_autoriza", TypeName = "int(11)")]
    public int? IdUsuarioAutoriza { get; set; }

    [Column("fecha_autorizacion", TypeName = "datetime")]
    public DateTime? FechaAutorizacion { get; set; }

    [Column("orden_ejecucion", TypeName = "int(11)")]
    public int OrdenEjecucion { get; set; }

    [Column("costo_mano_obra")]
    [Precision(10, 2)]
    public decimal? CostoManoObra { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime FechaCreacion { get; set; }

    [Column("fecha_finalizacion", TypeName = "datetime")]
    public DateTime? FechaFinalizacion { get; set; }

    [ForeignKey("IdDiagnostico")]
    [InverseProperty("AccionesReparacions")]
    public virtual Diagnostico IdDiagnosticoNavigation { get; set; } = null!;

    [ForeignKey("IdEstado")]
    [InverseProperty("AccionesReparacions")]
    public virtual EstadosAccion IdEstadoNavigation { get; set; } = null!;

    [ForeignKey("IdUsuarioAutoriza")]
    [InverseProperty("AccionesReparacions")]
    public virtual Usuario? IdUsuarioAutorizaNavigation { get; set; }

    [InverseProperty("IdAccionNavigation")]
    public virtual ICollection<UsoRepuesto> UsoRepuestos { get; set; } = new List<UsoRepuesto>();

    [ForeignKey("IdAccion")]
    [InverseProperty("IdAccions")]
    public virtual ICollection<AccionesReparacion> IdAccionPrerequisitos { get; set; } = new List<AccionesReparacion>();

    [ForeignKey("IdAccionPrerequisito")]
    [InverseProperty("IdAccionPrerequisitos")]
    public virtual ICollection<AccionesReparacion> IdAccions { get; set; } = new List<AccionesReparacion>();
}
