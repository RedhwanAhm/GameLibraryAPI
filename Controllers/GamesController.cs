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
    public class GamesController : ControllerBase
    {
        private readonly GameDBContext _context;

        public GamesController(GameDBContext context)
        {
            _context = context;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            return await _context.Game.ToListAsync();
        }

        // POST: api/Games
        // POST: api/Games
        [HttpPost]
        public async Task<ActionResult<Game>> CreateGame(GameDTO gameDTO)
        {
            var genre = await _context.Genre.FindAsync(gameDTO.GenreId);

            if (genre == null)
            {
                // If the genre doesn't exist, create a new genre
                genre = new Genre { Name = gameDTO.GenreName };
                _context.Genre.Add(genre);
            }

            var game = new Game
            {
                Title = gameDTO.Title,
                GenreID = genre.Id
            };

            genre.Games.Add(game); // Add the game to the genre's Games collection

            _context.Game.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGames), new { id = game.Id }, game);
        }
    }
}