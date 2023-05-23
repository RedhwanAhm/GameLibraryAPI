using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly GameDBContext _context;

        public GenresController(GameDBContext context)
        {
            _context = context;
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenres()
        {
            return await _context.Genre.ToListAsync();
        }

        // GET: api/Genres/{id}/Games
        [HttpGet("{id}/Games")]
        public async Task<ActionResult<IEnumerable<Game>>> GetGamesByGenre(int id)
        {
            var games = await _context.Game.Where(g => g.GenreID == id).ToListAsync();

            if (games == null)
            {
                return NotFound();
            }

            return games;
        }

         // POST: api/Genres
        [HttpPost]
        public async Task<ActionResult<Genre>> CreateGenre(Genre genre)
        {
            genre.Games = new List<Game>(); // Create an empty list of games for the genre

            _context.Genre.Add(genre);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenres", new { id = genre.Id }, genre);
        }

        [HttpPost("{id}/GetName")]
        public async Task<ActionResult<string>> GetGenreName(int id)
        {
            var genre = await _context.Genre.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            return genre.Name;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, Genre updatedGenre)
        {
            if (id != updatedGenre.Id)
            {
                return BadRequest();
            }

            var genre = await _context.Genre.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            genre.Name = updatedGenre.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool GenreExists(int id)
        {
            return _context.Genre.Any(g => g.Id == id);
        }
    }
}