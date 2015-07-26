﻿using System.Collections.Generic;
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
        public async Task<IHttpActionResult> Get(int userId, string gameTitle, string platformId, string genreId, double distance, int? take, int? skip)
        {
            if (!take.HasValue) take = 10000;
            if (!skip.HasValue) skip = 0;

            // Posizione utente corrente
            DbGeography position = db.Users
                                     .Where(u => u.UserId == userId)
                                     .Select(u => u.PositionUser)
                                     .FirstOrDefault();

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
                      .Where(u => u.User.UserId != userId)
                      .Where(u => u.User.PositionUser.Distance(position) <= distance)
                      .Where(g => string.IsNullOrEmpty(gameTitle) || g.Game.Title.Contains(gameTitle) )
                      .Where(gr => string.IsNullOrEmpty(genreId) || string.Equals(gr.Genre.GenreId, genreId))
                      .Where(p => string.IsNullOrEmpty(platformId) || string.Equals(p.Platform.PlatformId, platformId))
                      .Select(x => new SearchGame()
                      {
                          Status = x.LibraryComponent.LibraryComponents.Status,
                          GameLanguage = x.LibraryComponent.LibraryComponents.GameLanguage,
                          Genre = x.Game.Genre, 
                          Platform = x.Game.Platform,
                          Distance = x.User.PositionUser.Distance(position)                                             
                      });

            SearchGames result = new SearchGames()
            {
                SearchGame = q.OrderBy(u => u.Distance)
                              .Take(take.Value)
                              .Skip(skip.Value)
                              .ToList(),
                Count = q.Count()
            };

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
