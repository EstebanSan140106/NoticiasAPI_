using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoticiasAPI.Entities
{
    public class Noticia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contenido es requerido")]
        [Column(TypeName = "text")]
        public string Contenido { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "El resumen no puede exceder 1000 caracteres")]
        public string? Resumen { get; set; }

        [StringLength(500, ErrorMessage = "La URL de imagen no puede exceder 500 caracteres")]
        public string? ImagenUrl { get; set; }

        [StringLength(100, ErrorMessage = "El autor no puede exceder 100 caracteres")]
        public string? Autor { get; set; }

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        public bool Activa { get; set; } = true;

        public bool Destacada { get; set; } = false;

        public int Visualizaciones { get; set; } = 0;

        // Clave foránea
        [ForeignKey("Categoria")]
        public int CategoriaId { get; set; }

        // Propiedad de navegación
        public virtual Categoria Categoria { get; set; } = null!;
    }
}