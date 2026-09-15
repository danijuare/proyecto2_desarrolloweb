using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace proyecto_2_desarrollo_web.Models;

[Table("categorias_repuesto")]
[Index("Nombre", Name = "nombre", IsUnique = true)]
public partial class CategoriasRepuesto
{
    [Key]
    [Column("id_categoria", TypeName = "int(11)")]
    public int IdCategoria { get; set; }

    [Column("nombre")]
    [StringLength(60)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdCategoriaNavigation")]
    public virtual ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
}
