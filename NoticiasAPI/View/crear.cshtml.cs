using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NoticiasAPI.Context;
using NoticiasAPI.Entities;
using NoticiasAPI.DTO; // Si usas DTOs como ViewModels de entrada

namespace NoticiasWebApp.Pages.Noticias
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CrearNtoticiaDto NoticiaInput { get; set; } = default!; // Usar DTO para la entrada

        public async Task<IActionResult> OnPostAsync()
        {
            // Aquí, el ModelState ya validará si los campos de NoticiaInput están requeridos, etc.
            if (!ModelState.IsValid)
            {
                return Page(); // Vuelve a mostrar el formulario con errores
            }

            var noticia = new Noticia
            {
                Titulo = NoticiaInput.Titulo,
                Contenido = NoticiaInput.Contenido,
                Autor = NoticiaInput.Autor,
                Categoria = NoticiaInput.Categoria,
                FechaPublicacion = DateTime.Now // Asignar la fecha en el servidor
            };

            _context.Noticias.Add(noticia);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index"); // Redirigir a la lista después de crear
        }
    }
}