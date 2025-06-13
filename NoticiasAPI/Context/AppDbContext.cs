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
            // Inicializar Categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Política" },
                new Categoria { Id = 2, Nombre = "Economía" },
                new Categoria { Id = 3, Nombre = "Tecnología" },
                new Categoria { Id = 4, Nombre = "Deportes" },
                new Categoria { Id = 5, Nombre = "Ciencia" },
                new Categoria { Id = 6, Nombre = "Cultura" }
            );

            // Inicializar Noticias
            modelBuilder.Entity<Noticia>().HasData(
                // Política
                new Noticia
                {
                    Id = 1,
                    Titulo = "Trump prohíbe viajes a EEUU desde 12 países, alegando preocupaciones de seguridad",
                    Autor = "Jeff Mason y Nandita Bose",
                    FechaPublicacion = DateTime.Parse("2025-06-05T02:21:00Z"),
                    Medio = "Reuters",
                    CategoriaId = 1
                },
                new Noticia
                {
                    Id = 2,
                    Titulo = "Aumenta la tensión ante un posible ataque de Israel contra Irán sin la participación de Estados Unidos",
                    Autor = "Luis de Vega",
                    FechaPublicacion = DateTime.Parse("2025-06-12T12:11:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 1
                },
                // Economía
                new Noticia
                {
                    Id = 3,
                    Titulo = "Inflación argentina alcanzaría 28,6% en 2025, con crecimiento de PIB de 5,1%: sondeo banco central",
                    Autor = "Nicolás Misculin",
                    FechaPublicacion = DateTime.Parse("2025-06-05T21:16:00Z"),
                    Medio = "Reuters",
                    CategoriaId = 2
                },
                new Noticia
                {
                    Id = 4,
                    Titulo = "ANÁLISIS Pese a repunte de inflación, se espera que banco central de México recorte tasa en 50 pb",
                    Autor = "Noé Torres",
                    FechaPublicacion = DateTime.Parse("2025-06-10T16:10:00Z"),
                    Medio = "Reuters",
                    CategoriaId = 2
                },
                new Noticia
                {
                    Id = 5,
                    Titulo = "Inflación interanual de México supera en mayo objetivo oficial",
                    Autor = "Noé Torres",
                    FechaPublicacion = DateTime.Parse("2025-06-09T12:54:00Z"),
                    Medio = "Reuters",
                    CategoriaId = 2
                },
                // Tecnología
                new Noticia
                {
                    Id = 6,
                    Titulo = "IBM anuncia el primer superordenador cuántico a gran escala y tolerante a fallos",
                    Autor = "Raúl Limón",
                    FechaPublicacion = DateTime.Parse("2025-06-10T10:00:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 3
                },
                new Noticia
                {
                    Id = 7,
                    Titulo = "WWDC 2025: Apple presenta el primer rediseño de sus sistemas operativos desde 2013",
                    Autor = "Isabel Rubio",
                    FechaPublicacion = DateTime.Parse("2025-06-09T19:16:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 3
                },
                // Deportes
                new Noticia
                {
                    Id = 8,
                    Titulo = "Mundial de Clubes de la FIFA 2025 en Estados Unidos: estadios, fase de grupos y calendario",
                    Autor = "Alonso Martínez",
                    FechaPublicacion = DateTime.Parse("2025-06-12T15:48:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 4
                },
                new Noticia
                {
                    Id = 9,
                    Titulo = "Alcaraz and Sinner French Open final scaled new heights, agree former champions",
                    Autor = "Reuters",
                    FechaPublicacion = DateTime.Parse("2025-06-09T12:22:00Z"),
                    Medio = "Reuters",
                    CategoriaId = 4
                },
                // Ciencia
                new Noticia
                {
                    Id = 10,
                    Titulo = "Una sonda europea capta las primeras imágenes del polo sur del Sol",
                    Autor = "Nuño Domínguez",
                    FechaPublicacion = DateTime.Parse("2025-06-11T14:00:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 5
                },
                new Noticia
                {
                    Id = 11,
                    Titulo = "El eslabón perdido en la evolución del ‘Tyrannosaurus rex’ vivió en Mongolia hace millones de años",
                    Autor = "Constanza Cabrera",
                    FechaPublicacion = DateTime.Parse("2025-06-11T14:59:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 5
                },
                // Cultura
                new Noticia
                {
                    Id = 12,
                    Titulo = "Muere Brian Wilson, visionario del pop y líder de The Beach Boys, a los 82 años",
                    Autor = "Carlos Marcos",
                    FechaPublicacion = DateTime.Parse("2025-06-11T17:04:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 6
                },
                new Noticia
                {
                    Id = 13,
                    Titulo = "Muere Harris Yulin, el eterno actor secundario de películas como ‘Scarface’ y ‘Training Day’",
                    Autor = "El País",
                    FechaPublicacion = DateTime.Parse("2025-06-12T07:21:00Z"), // Convertido de CEST a UTC
                    Medio = "El País",
                    CategoriaId = 6
                }
            );
        }
    }
}
