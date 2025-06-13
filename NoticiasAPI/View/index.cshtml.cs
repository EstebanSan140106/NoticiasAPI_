using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Context; 
using NoticiasAPI.Entities; 
using NoticiasAPI.DTO;
using Microsoft.AspNetCore.Mvc;

namespace NoticiasWebApp.Pages.Noticias
{
	public class IndexModel : PageModel
	{
		private readonly AppDbContext _context;

		public IndexModel(AppDbContext context)
		{
			_context = context;
		}

		public IList<Noticia> Noticias { get; set; } = default!; // O IList<NoticiaDto>

		[BindProperty(SupportsGet = true)] // Para que el término de búsqueda se vincule
		public string? SearchTerm { get; set; }

		public async Task OnGetAsync()
		{
			IQueryable<Noticia> noticiasQuery = _context.Noticias;

			if (!string.IsNullOrEmpty(SearchTerm))
			{
				string searchLower = SearchTerm.ToLower();
				noticiasQuery = noticiasQuery.Where(n =>
					n.Titulo.ToLower().Contains(searchLower) ||
					n.Contenido.ToLower().Contains(searchLower) ||
					n.Autor.ToLower().Contains(searchLower) ||
					n.Categoria.ToLower().Contains(searchLower));
			}

			Noticias = await noticiasQuery.ToListAsync();
		}

		// Para manejar la eliminación vía POST (formularios tradicionales)
		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			var noticia = await _context.Noticias.FindAsync(id);

			if (noticia != null)
			{
				_context.Noticias.Remove(noticia);
				await _context.SaveChangesAsync();
			}

			return RedirectToPage("./Index"); // Redirigir a la misma página para actualizar la lista
		}
	}
}
