namespace NoticiasAPI.DTO
{
    public class CrearNtoticiaDto
    {
        public string Titulo { get; set; } 
        public string Contenido { get; set; }

        public string Autor { get; set; }

        public string Categoria { get; set; }

        public int CategoriaId { get; set; }
        public DateTime FechaPublicacion { get; set; } 
    }
}
