﻿namespace NoticiasAPI.Entities
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Categoria> Categorias { get; set; }
    }
}
