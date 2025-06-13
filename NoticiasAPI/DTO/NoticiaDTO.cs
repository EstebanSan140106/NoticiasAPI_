namespace NoticiasAPI.DTO
{
    public class NoticiaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Medio { get; set; }
        public string CategoriaNombre { get; set; }
    }
}
