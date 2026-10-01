using System.ComponentModel.DataAnnotations;

namespace proyecto_2_desarrollo_web.DTOs
{
    // DTO para respuestas (GET)
    public class CategoriaRepuestoResponseDto
    {
        public int id_categoria { get; set; }
        public string nombre { get; set; } = string.Empty;
    }

    // DTO para creación y actualización (POST/PUT)
    public class CategoriaRepuestoCreateUpdateDto
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(60, ErrorMessage = "El nombre no puede exceder los 60 caracteres.")]
        public string nombre { get; set; } = string.Empty;
    }
}