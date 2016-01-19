using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using uPlayAgain.Data.Dto;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Controllers
{
    public class SearchController : BaseController
    {
        // GET: api/Search/5
        public async Task<IHttpActionResult> Get(int userId, string gameTitle, string platformId, string genreId, double distance, int? take, int? skip)
        {
            if (!take.HasValue) take = 10000;
            if (!skip.HasValue) skip = 0;

            // Posizione utente corrente
            DbGeography position = await db.Users
                                           .Where(u => u.UserId == userId)
                                           .Select(u => u.PositionUser)
                                           .FirstOrDefaultAsync();

            var q = db.Libraries
                      .Join(db.Users,
                          l => l.UserId,
                          u => u.Id,
                          (l, u) => new { User = u, Library = l })                          
                      .Join(db.LibraryComponents,
                          l => l.Library.LibraryId,
                          lc => lc.LibraryId,
                          (l, lc) => new { Library = l, LibraryComponents = lc, l.User })
                      .Join(db.Games,
                          lc => lc.LibraryComponents.GameId,
                          g => g.GameId,
                          (lc, g) => new { LibraryComponent = lc, Game = g, g.Genre, g.Platform, lc.User, lc.Library })
                      //Gioco scambiabile e non cancellati
                      .Where(l => l.LibraryComponent.LibraryComponents.IsExchangeable && !l.LibraryComponent.LibraryComponents.IsDeleted)

                      .Where(u => u.User.UserId != userId)
                      .Where(u => u.User.PositionUser.Distance(position) <= distance * 1000)
                      .Where(g => string.IsNullOrEmpty(gameTitle) || g.Game.Title.Contains(gameTitle) )                      
                      .Where(gr => string.IsNullOrEmpty(genreId) || string.Equals(gr.Genre.GenreId, genreId))
                      .Where(p => string.IsNullOrEmpty(platformId) || string.Equals(p.Platform.PlatformId, platformId))
                      .Select(x => new SearchGame()
                      {
                          
                          Status = x.LibraryComponent.LibraryComponents.Status,
                          GameLanguage = x.LibraryComponent.LibraryComponents.GameLanguage,
                          Genre = x.Game.Genre, 
                          Platform = x.Game.Platform,
                          Game = new GameDto()
                          {
                              Description = x.Game.Description,
                              GameId = x.Game.GameId,
                              GenreId = x.Game.GenreId,
                              Image = x.Game.ImageThumb,
                              ImportId = x.Game.ImportId,
                              PlatformId = x.Game.PlatformId,
                              ShortName = x.Game.ShortName,
                              RegistrationDate = x.Game.RegistrationDate,
                              Title = x.Game.Title
                          },
                          User = x.User,
                          LibraryComponent = x.LibraryComponent.LibraryComponents,
                          Distance = (x.User.PositionUser.Distance(position) / 1000)
                      });

            SearchGames result = new SearchGames()
            {
                SearchGame = await q.OrderBy(u => u.Distance)
                                    .Take(take.Value)
                                    .Skip(skip.Value)
                                    .ToListAsync(),
                Count = await q.CountAsync()
            };

            return Ok(result);
        }

        // TODO --> fare ricerca per il gioco semplice ma con i parametri
        [HttpGet]
        [Route("api/Games/Search/")]
        [Route("api/Games/Search/{gameTitle?}/")]
        [Route("api/Games/Search/{gameTitle?}/{platformId?}")]
        [Route("api/Games/Search/{gameTitle?}/{platformId?}/{genreId?}")]
        [ResponseType(typeof(Game))]
        public IQueryable<Game> SearchGame(string gameTitle = "", string platformId = "", string genreId = "")
        {
            return db.Games
                     .Include(p => p.Platform)
                     .Include(p => p.Genre)
                     .Where(g => string.IsNullOrEmpty(gameTitle) || g.Title.Contains(gameTitle))
                     .Where(gr => string.IsNullOrEmpty(genreId) || string.Equals(gr.Genre.GenreId, genreId))
                     .Where(p => string.IsNullOrEmpty(platformId) || string.Equals(p.Platform.PlatformId, platformId));            
        }        
    }
}
