using System.ComponentModel.DataAnnotations;

namespace NoticiasAPI.Entities
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activa { get; set; } = true;

        // Relación con Noticias
        public virtual ICollection<Noticia> Noticias { get; set; } = new List<Noticia>();
    }
}
