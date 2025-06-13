using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Context;
using NoticiasAPI.Entities;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCategorias()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Where(c => c.Activa)
                    .Select(c => new
                    {
                        c.Id,
                        c.Nombre,
                        c.Descripcion,
                        c.FechaCreacion,
                        c.Activa,
                        TotalNoticias = c.Noticias.Count(n => n.Activa)
                    })
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .Where(c => c.Id == id && c.Activa)
                    .Select(c => new
                    {
                        c.Id,
                        c.Nombre,
                        c.Descripcion,
                        c.FechaCreacion,
                        c.Activa,
                        TotalNoticias = c.Noticias.Count(n => n.Activa),
                        Noticias = c.Noticias.Where(n => n.Activa).Select(n => new
                        {
                            n.Id,
                            n.Titulo,
                            n.Resumen,
                            n.FechaPublicacion,
                            n.Visualizaciones,
                            n.Destacada
                        }).OrderByDescending(n => n.FechaPublicacion).Take(10)
                    })
                    .FirstOrDefaultAsync();

                if (categoria == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Categorias/activas
        [HttpGet("activas")]
        public async Task<ActionResult<IEnumerable<object>>> GetCategoriasActivas()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Where(c => c.Activa)
                    .Select(c => new
                    {
                        c.Id,
                        c.Nombre,
                        c.Descripcion
                    })
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si ya existe una categoría con el mismo nombre
                var existeCategoria = await _context.Categorias
                    .AnyAsync(c => c.Nombre.ToLower() == categoria.Nombre.ToLower());

                if (existeCategoria)
                {
                    return Conflict(new { message = "Ya existe una categoría con este nombre" });
                }

                categoria.FechaCreacion = DateTime.Now;
                categoria.Activa = true;

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCategoria", new { id = categoria.Id }, categoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PUT: api/Categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            try
            {
                if (id != categoria.Id)
                {
                    return BadRequest(new { message = "El ID no coincide" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si existe otra categoría con el mismo nombre
                var existeCategoria = await _context.Categorias
                    .AnyAsync(c => c.Id != id && c.Nombre.ToLower() == categoria.Nombre.ToLower());

                if (existeCategoria)
                {
                    return Conflict(new { message = "Ya existe una categoría con este nombre" });
                }

                _context.Entry(categoria).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Categoría actualizada correctamente" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .Include(c => c.Noticias)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (categoria == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                // Verificar si tiene noticias asociadas
                if (categoria.Noticias.Any())
                {
                    return BadRequest(new { message = "No se puede eliminar la categoría porque tiene noticias asociadas. Desactívela en su lugar." });
                }

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Categoría eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PUT: api/Categorias/5/toggle
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleCategoria(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                categoria.Activa = !categoria.Activa;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Categoría {(categoria.Activa ? "activada" : "desactivada")} correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
}