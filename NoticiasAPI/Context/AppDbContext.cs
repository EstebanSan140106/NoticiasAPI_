using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Entities;

namespace NoticiasAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Noticia> Noticias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.Nombre).IsUnique();

                entity.ToTable("Categorias");
            });

            // Configuración de Noticia
            modelBuilder.Entity<Noticia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Contenido).IsRequired().HasColumnType("text");
                entity.Property(e => e.Resumen).HasMaxLength(1000);
                entity.Property(e => e.ImagenUrl).HasMaxLength(500);
                entity.Property(e => e.Autor).HasMaxLength(100);
                entity.Property(e => e.FechaCreacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.FechaPublicacion).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Activa).HasDefaultValue(true);
                entity.Property(e => e.Destacada).HasDefaultValue(false);
                entity.Property(e => e.Visualizaciones).HasDefaultValue(0);

                // Relación con Categoria
                entity.HasOne(d => d.Categoria)
                      .WithMany(p => p.Noticias)
                      .HasForeignKey(d => d.CategoriaId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_Noticias_Categorias");

                // Índices
                entity.HasIndex(e => e.CategoriaId).HasDatabaseName("IX_Noticias_CategoriaId");
                entity.HasIndex(e => e.FechaPublicacion).HasDatabaseName("IX_Noticias_FechaPublicacion");
                entity.HasIndex(e => e.Activa).HasDatabaseName("IX_Noticias_Activa");

                entity.ToTable("Noticias");
            });

            // Datos iniciales (Seed Data)
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 1,
                    Nombre = "Tecnología",
                    Descripcion = "Noticias sobre avances tecnológicos, gadgets e innovación",
                    FechaCreacion = DateTime.Now,
                    Activa = true
                },
                new Categoria
                {
                    Id = 2,
                    Nombre = "Deportes",
                    Descripcion = "Noticias deportivas, resultados y eventos",
                    FechaCreacion = DateTime.Now,
                    Activa = true
                },
                new Categoria
                {
                    Id = 3,
                    Nombre = "Política",
                    Descripcion = "Noticias políticas nacionales e internacionales",
                    FechaCreacion = DateTime.Now,
                    Activa = true
                },
                new Categoria
                {
                    Id = 4,
                    Nombre = "Entretenimiento",
                    Descripcion = "Noticias de entretenimiento, celebridades y espectáculos",
                    FechaCreacion = DateTime.Now,
                    Activa = true
                },
                new Categoria
                {
                    Id = 5,
                    Nombre = "Salud",
                    Descripcion = "Noticias sobre salud, medicina y bienestar",
                    FechaCreacion = DateTime.Now,
                    Activa = true
                }
            );
        }

        public override int SaveChanges()
        {
            ActualizarFechas();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ActualizarFechas();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ActualizarFechas()
        {
            var entidades = ChangeTracker.Entries()
                .Where(x => x.Entity is Noticia && x.State == EntityState.Modified)
                .Select(x => x.Entity as Noticia);

            foreach (var entidad in entidades)
            {
                if (entidad != null)
                {
                    entidad.FechaActualizacion = DateTime.Now;
                }
            }
        }
    }
}