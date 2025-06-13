using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Context;
using NoticiasAPI.Entities;
using NoticiasAPI.DTO;

namespace NoticiasWebApp.Pages.Noticias
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ActualizarNoticiaDto NoticiaInput { get; set; } = default!;

        [BindProperty] // Para obtener el ID de la URL
        public int NoticiaId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noticia = await _context.Noticias.FirstOrDefaultAsync(m => m.Id == id);
            if (noticia == null)
            {
                return NotFound();
            }

            // Mapear la entidad a tu DTO (o ViewModel de edición)
            NoticiaId = noticia.Id;
            NoticiaInput = new ActualizarNoticiaDto
            {
                Titulo = noticia.Titulo,
                Contenido = noticia.Contenido,
                Autor = noticia.Autor,
                Categoria = noticia.Categoria
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var noticiaToUpdate = await _context.Noticias.FindAsync(NoticiaId);

            if (noticiaToUpdate == null)
            {
                return NotFound();
            }

            // Actualizar las propiedades de la entidad con los datos del DTO
            noticiaToUpdate.Titulo = NoticiaInput.Titulo;
            noticiaToUpdate.Contenido = NoticiaInput.Contenido;
            noticiaToUpdate.Autor = NoticiaInput.Autor;
            noticiaToUpdate.Categoria = NoticiaInput.Categoria;

            _context.Entry(noticiaToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoticiaExists(NoticiaId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool NoticiaExists(int id)
        {
            return _context.Noticias.Any(e => e.Id == id);
        }
    }
}