using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Context;
using NoticiasAPI.Entities;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticiaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NoticiaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Noticias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetNoticias([FromQuery] int? categoriaId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Noticias
                    .Include(n => n.Categoria)
                    .Where(n => n.Activa);

                if (categoriaId.HasValue)
                {
                    query = query.Where(n => n.CategoriaId == categoriaId.Value);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var noticias = await query
                    .Select(n => new
                    {
                        n.Id,
                        n.Titulo,
                        n.Resumen,
                        n.ImagenUrl,
                        n.Autor,
                        n.FechaPublicacion,
                        n.Visualizaciones,
                        n.Destacada,
                        Categoria = new
                        {
                            n.Categoria.Id,
                            n.Categoria.Nombre
                        }
                    })
                    .OrderByDescending(n => n.FechaPublicacion)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    data = noticias,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = totalPages
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Noticias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetNoticia(int id)
        {
            try
            {
                var noticia = await _context.Noticias
                    .Include(n => n.Categoria)
                    .Where(n => n.Id == id && n.Activa)
                    .Select(n => new
                    {
                        n.Id,
                        n.Titulo,
                        n.Contenido,
                        n.Resumen,
                        n.ImagenUrl,
                        n.Autor,
                        n.FechaPublicacion,
                        n.FechaCreacion,
                        n.FechaActualizacion,
                        n.Visualizaciones,
                        n.Destacada,
                        Categoria = new
                        {
                            n.Categoria.Id,
                            n.Categoria.Nombre,
                            n.Categoria.Descripcion
                        }
                    })
                    .FirstOrDefaultAsync();

                if (noticia == null)
                {
                    return NotFound(new { message = "Noticia no encontrada" });
                }

                // Incrementar visualizaciones
                var noticiaEntity = await _context.Noticias.FindAsync(id);
                if (noticiaEntity != null)
                {
                    noticiaEntity.Visualizaciones++;
                    await _context.SaveChangesAsync();
                }

                return Ok(noticia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Noticias/categoria/5
        [HttpGet("categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetNoticiasPorCategoria(int categoriaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(categoriaId);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categoría no encontrada" });
                }

                var query = _context.Noticias
                    .Include(n => n.Categoria)
                    .Where(n => n.CategoriaId == categoriaId && n.Activa);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var noticias = await query
                    .Select(n => new
                    {
                        n.Id,
                        n.Titulo,
                        n.Resumen,
                        n.ImagenUrl,
                        n.Autor,
                        n.FechaPublicacion,
                        n.Visualizaciones,
                        n.Destacada
                    })
                    .OrderByDescending(n => n.FechaPublicacion)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    categoria = new { categoria.Id, categoria.Nombre, categoria.Descripcion },
                    data = noticias,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = totalPages
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Noticias/destacadas
        [HttpGet("destacadas")]
        public async Task<ActionResult<IEnumerable<object>>> GetNoticiasDestacadas([FromQuery] int limit = 5)
        {
            try
            {
                var noticias = await _context.Noticias
                    .Include(n => n.Categoria)
                    .Where(n => n.Activa && n.Destacada)
                    .Select(n => new
                    {
                        n.Id,
                        n.Titulo,
                        n.Resumen,
                        n.ImagenUrl,
                        n.Autor,
                        n.FechaPublicacion,
                        n.Visualizaciones,
                        Categoria = new
                        {
                            n.Categoria.Id,
                            n.Categoria.Nombre
                        }
                    })
                    .OrderByDescending(n => n.FechaPublicacion)
                    .Take(limit)
                    .ToListAsync();

                return Ok(noticias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Noticias/recientes
        [HttpGet("recientes")]
        public async Task<ActionResult<IEnumerable<object>>> GetNoticiasRecientes([FromQuery] int limit = 10)
        {
            try
            {
                var noticias = await _context.Noticias
                    .Include(n => n.Categoria)
                    .Where(n => n.Activa)
                    .Select(n => new
                    {
                        n.Id,
                        n.Titulo,
                        n.Resumen,
                        n.ImagenUrl,
                        n.Autor,
                        n.FechaPublicacion,
                        n.Visualizaciones,
                        n.Destacada,
                        Categoria = new
                        {
                            n.Categoria.Id,
                            n.Categoria.Nombre
                        }
                    })
                    .OrderByDescending(n => n.FechaPublicacion)
                    .Take(limit)
                    .ToListAsync();

                return Ok(noticias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/Noticias/buscar
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarNoticias([FromQuery] string termino, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                {
                    return BadRequest(new { message = "El término de búsqueda es requerido" });
                }

                var query = _context.Noticias
                    .Include(n => n.Categoria)
                    .Where(n => n.Activa &&
                        (n.Titulo.Contains(termino) ||
                         n.Contenido.Contains(termino) ||
                         n.Resumen.Contains(termino) ||
                         n.Categoria.Nombre.Contains(termino)));

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var noticias = await query
                    .Select(n => new
                    {
                        n.Id,
                        n.Titulo,
                        n.Resumen,
                        n.ImagenUrl,
                        n.Autor,
                        n.FechaPublicacion,
                        n.Visualizaciones,
                        n.Destacada,
                        Categoria = new
                        {
                            n.Categoria.Id,
                            n.Categoria.Nombre
                        }
                    })
                    .OrderByDescending(n => n.FechaPublicacion)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var response = new
                {
                    termino = termino,
                    data = noticias,
                    pagination = new
                    {
                        currentPage = page,
                        pageSize = pageSize,
                        totalCount = totalCount,
                        totalPages = totalPages
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // POST: api/Noticias
        [HttpPost]
        public async Task<ActionResult<Noticia>> PostNoticia(Noticia noticia)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar que la categoría existe
                var categoria = await _context.Categorias.FindAsync(noticia.CategoriaId);
                if (categoria == null)
                {
                    return BadRequest(new { message = "La categoría especificada no existe" });
                }

                noticia.FechaCreacion = DateTime.Now;
                noticia.FechaPublicacion = DateTime.Now;
                noticia.Activa = true;
                noticia.Visualizaciones = 0;

                _context.Noticias.Add(noticia);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetNoticia", new { id = noticia.Id }, noticia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PUT: api/Noticias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNoticia(int id, Noticia noticia)
        {
            try
            {
                if (id != noticia.Id)
                {
                    return BadRequest(new { message = "El ID no coincide" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar que la noticia existe
                var noticiaExistente = await _context.Noticias.FindAsync(id);
                if (noticiaExistente == null)
                {
                    return NotFound(new { message = "Noticia no encontrada" });
                }

                // Verificar que la categoría existe
                var categoria = await _context.Categorias.FindAsync(noticia.CategoriaId);
                if (categoria == null)
                {
                    return BadRequest(new { message = "La categoría especificada no existe" });
                }

                // Actualizar los campos
                noticiaExistente.Titulo = noticia.Titulo;
                noticiaExistente.Contenido = noticia.Contenido;
                noticiaExistente.Resumen = noticia.Resumen;
                noticiaExistente.ImagenUrl = noticia.ImagenUrl;
                noticiaExistente.Autor = noticia.Autor;
                noticiaExistente.CategoriaId = noticia.CategoriaId;
                noticiaExistente.Destacada = noticia.Destacada;
                noticiaExistente.FechaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Noticia actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // PUT: api/Noticias/5/toggle
        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleNoticia(int id)
        {
            try
            {
                var noticia = await _context.Noticias.FindAsync(id);
                if (noticia == null)
                {
                    return NotFound(new { message = "Noticia no encontrada" });
                }

                noticia.Activa = !noticia.Activa;
                noticia.FechaActualizacion = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Noticia {(noticia.Activa ? "activada" : "desactivada")} correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // DELETE: api/Noticias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoticia(int id)
        {
            try
            {
                var noticia = await _context.Noticias.FindAsync(id);
                if (noticia == null)
                {
                    return NotFound(new { message = "Noticia no encontrada" });
                }

                _context.Noticias.Remove(noticia);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Noticia eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // Método auxiliar para verificar si existe una noticia
        private bool NoticiaExists(int id)
        {
            return _context.Noticias.Any(e => e.Id == id);
        }
    }
}