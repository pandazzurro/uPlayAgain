using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using uPlayAgain.Dto;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class GamesController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Games
        public IQueryable<Game> GetGames()
        {
            return db.Games;
        }

        [Route("api/Games/Resize")]        
        public async Task GetResize()
        {
            foreach (Game g in db.Games.Where(x => x.ImageThumb == null))
            {
                g.ImageThumb = g.Resize();
                db.Entry(g).State = EntityState.Modified;
            }
            await db.SaveChangesAsync();
        }

        [Route("api/Games/Last/{number:int}")]
        public IQueryable GetLastGame(int number)
        {
            return db.Games
                     .OrderByDescending(t => t.RegistrationDate)
                     .Include(t => t.Platform)
                     .Include(t => t.Genre)
                     .Take(number)
                     .Select( x => 
                        new
                        {
                            Image = x.Image,
                            Title = x.Title
                        });
        }

        // GET: api/Games/5
        [ResponseType(typeof(GameDto))]
        public async Task<IHttpActionResult> GetGame(int id)
        {
            GameDto game = await db.Games.Where(x => x.GameId == id).Select(x => new GameDto()
            {
                Description= x.Description,
                GameId = x.GameId,                
                GenreId = x.GenreId,
                Image = x.ImageThumb,
                ImportId = x.ImportId,                
                PlatformId = x.PlatformId,
                ShortName = x.ShortName,
                RegistrationDate = x.RegistrationDate,
                Title = x.Title
            }).SingleOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }
            
            return Ok(game);
        }

        // GET: api/Games/Image/5
        [Route("api/Games/Image/{id:int}")]
        [ResponseType(typeof(byte[]))]
        public async Task<IHttpActionResult> GetGameImage(int id)
        {
            Game game = await db.Games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(new GameImageDto() { Image = game.Image });
        }

        // PUT: api/Games/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGame(int id, Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != game.GameId)
            {
                return BadRequest();
            }

            db.Entry(await CheckGenrePlatform(game)).State = EntityState.Modified;
            
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Games
        [ResponseType(typeof(Game))]
        public async Task<IHttpActionResult> PostGame(Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Calcolo le immagini thum
            if(game.Image != null) game.ImageThumb = game.Resize();
            db.Games.Add(await CheckGenrePlatform(game));

            await db.SaveChangesAsync();
            
            return CreatedAtRoute("DefaultApi", new { id = game.GameId }, game);
        }

        // DELETE: api/Games/5
        [ResponseType(typeof(Game))]
        public async Task<IHttpActionResult> DeleteGame(int id)
        {
            Game game = await db.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            db.Games.Remove(game);
            await db.SaveChangesAsync();

            return Ok(game);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.GameId == id) > 0;
        }

        private async Task<Game> CheckGenrePlatform(Game game)
        {
            if (game.Genre != null)
            {
                if (await db.Genres.Where(gr => gr.GenreId == game.Genre.GenreId).AnyAsync()) game.Genre = null;
                else Conflict();
            }

            if (game.Platform != null)
            {
                if (await db.Platforms.Where(p => p.PlatformId == game.Platform.PlatformId).AnyAsync()) game.Platform = null;
                else Conflict();
            }

            return game;
        }
    }
}