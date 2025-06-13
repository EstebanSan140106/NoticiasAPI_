using Microsoft.AspNetCore.Mvc;

namespace NoticiasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticiasController : ControllerBase
    {
        private readonly INoticiaService _noticiaService;

        public NoticiasController(INoticiaService noticiaService)
        {
            _noticiaService = noticiaService;
        }

        // GET: api/Noticias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Noticia>>> GetNoticias()
        {
            try
            {
                var noticias = await _noticiaService.GetAllNoticiasAsync();
                return Ok(noticias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las noticias: {ex.Message}");
            }
        }

        // GET: api/Noticias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Noticia>> GetNoticia(int id)
        {
            try
            {
                var noticia = await _noticiaService.GetNoticiaByIdAsync(id);

                if (noticia == null)
                {
                    return NotFound($"Noticia con ID {id} no encontrada");
                }

                return Ok(noticia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener la noticia: {ex.Message}");
            }
        }

        // POST: api/Noticias
        [HttpPost]
        public async Task<ActionResult<Noticia>> CreateNoticia([FromBody] Noticia noticia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _noticiaService.CreateNoticiaAsync(noticia);
                return CreatedAtAction(nameof(GetNoticia), new { id = noticia.Id }, noticia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la noticia: {ex.Message}");
            }
        }

        // PUT: api/Noticias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNoticia(int id, [FromBody] Noticia noticia)
        {
            if (id != noticia.Id)
            {
                return BadRequest("El ID de la noticia no coincide");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingNoticia = await _noticiaService.GetNoticiaByIdAsync(id);
                if (existingNoticia == null)
                {
                    return NotFound($"Noticia con ID {id} no encontrada");
                }

                await _noticiaService.UpdateNoticiaAsync(noticia);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la noticia: {ex.Message}");
            }
        }

        // DELETE: api/Noticias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoticia(int id)
        {
            try
            {
                var noticia = await _noticiaService.GetNoticiaByIdAsync(id);
                if (noticia == null)
                {
                    return NotFound($"Noticia con ID {id} no encontrada");
                }

                await _noticiaService.DeleteNoticiaAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar la noticia: {ex.Message}");
            }
        }
    }

    // Noticia model (for reference)
    public class Noticia
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Autor { get; set; }
    }

    // Service interface (for reference)
    public interface INoticiaService
    {
        Task<IEnumerable<Noticia>> GetAllNoticiasAsync();
        Task<Noticia> GetNoticiaByIdAsync(int id);
        Task CreateNoticiaAsync(Noticia noticia);
        Task UpdateNoticiaAsync(Noticia noticia);
        Task DeleteNoticiaAsync(int id);
    }
}