using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Entities;

namespace NoticiasAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Noticia> Noticias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Noticia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Contenido).IsRequired();
                entity.Property(e => e.Autor).HasMaxLength(100);
                entity.Property(e => e.Categoria).HasMaxLength(50);
                entity.Property(e => e.Medio).HasMaxLength(100);  // ← NUEVA VALIDACIÓN
                entity.Property(e => e.FechaPublicacion).IsRequired();
            });
        }
    }
}