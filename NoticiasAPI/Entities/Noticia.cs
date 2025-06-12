namespace NoticiasAPI.Entities
{
    public class Noticia
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Autor { get; set; }
        public string Categoria { get; set; }
    }
}
