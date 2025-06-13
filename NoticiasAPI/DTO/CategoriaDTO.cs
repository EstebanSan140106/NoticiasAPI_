namespace NoticiasAPI.DTO
{
    public class CategoriaDTO
    {
        public int CatgegoriaId { get; set; }
        public string Nombre { get; set; }
        public List<CategoriaDTO> Categorias { get; set; }
    }
}
