using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using uPlayAgain.Dto;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class SearchController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Search/5
        public async Task<IHttpActionResult> Get(int userId, string gameTitle, string platformId, string genreId, double distance)
        {
            // Posizione utente corrente
            DbGeography position = db.Users
                                     .Where(u => u.UserId == userId)
                                     .Select(u => u.PositionUser)
                                     .FirstOrDefault();

            IList<SearchGame> result = db.Libraries
                                         .Join(db.Users,
                                               l => l.UserId,
                                               u => u.UserId,
                                               (u, l) => new { User = u, Library = l })
                          
                                         .Join(db.LibraryComponents,
                                               l => l.User.LibraryId,
                                               lc => lc.LibraryId,
                                               (l, lc) => new { LibraryComponents = lc, Library = l, l.User })
                                         .Join(db.Games,
                                               lc => lc.LibraryComponents.LibraryComponentId,
                                               g => g.GameId,
                                               (lc, g) => new { LibraryComponent = lc, Game = g, g.Genre, g.Platform, lc.User, lc.Library })
                                         .Where(u => u.User.User.PositionUser.Distance(position) <= distance)
                                         .Where(g => string.IsNullOrEmpty(gameTitle) || g.Game.Title.Contains(gameTitle) )
                                         .Where(gr => string.IsNullOrEmpty(genreId) && string.Compare(gr.Genre.GenreId, genreId, true) == 0)
                                         .Where(p => string.IsNullOrEmpty(platformId) && string.Compare(p.Platform.PlatformId, platformId, true) == 0)
                                         .Select(x => new SearchGame()
                                         {
                                             Status = x.LibraryComponent.LibraryComponents.Status,
                                             GameLanguage = x.LibraryComponent.LibraryComponents.GameLanguage,
                                             Genre = x.Game.Genre, 
                                             Platform = x.Game.Platform,
                                             Distance = x.User.User.PositionUser.Distance(position)
                                         })
                                         .OrderBy(u => u.Distance)
                                         .ToList();
            return Ok(result);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
