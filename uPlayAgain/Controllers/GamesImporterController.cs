using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using uPlayAgain.Data.EF.Context;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Controllers
{
    public class GamesImporterController : ODataController
    {
        private uPlayAgainContext db;
        public GamesImporterController()
        {
            db = new uPlayAgainContext();
        }
        

        // GET: odata/GamesImporter
        [EnableQuery]
        public IQueryable<Game> GetGamesImporter()
        {
            return db.Games;
        }

        // GET: odata/GamesImporter(5)
        [EnableQuery]
        public SingleResult<Game> GetGame([FromODataUri] int key)
        {
            return SingleResult.Create(db.Games.Where(game => game.GameId == key));
        }

        // PUT: odata/GamesImporter(5)
        public async Task<IHttpActionResult> Put([FromODataUri] int key, Game patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Game game = await db.Games.FindAsync(key);
            if (game == null)
            {
                return NotFound();
            }

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(game);
        }

        // POST: odata/GamesImporter
        public async Task<IHttpActionResult> Post(Game game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Games.Add(game);
            await db.SaveChangesAsync();

            return Created(game);
        }

        // PATCH: odata/GamesImporter(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<Game> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Game game = await db.Games.FindAsync(key);
            if (game == null)
            {
                return NotFound();
            }

            patch.Patch(game);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(game);
        }

        // DELETE: odata/GamesImporter(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            Game game = await db.Games.FindAsync(key);
            if (game == null)
            {
                return NotFound();
            }

            db.Games.Remove(game);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/GamesImporter(5)/Genre
        [EnableQuery]
        public SingleResult<Genre> GetGenre([FromODataUri] int key)
        {
            return SingleResult.Create(db.Games.Where(m => m.GameId == key).Select(m => m.Genre));
        }

        // GET: odata/GamesImporter(5)/Platform
        [EnableQuery]
        public SingleResult<Platform> GetPlatform([FromODataUri] int key)
        {
            return SingleResult.Create(db.Games.Where(m => m.GameId == key).Select(m => m.Platform));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameExists(int key)
        {
            return db.Games.Count(e => e.GameId == key) > 0;
        }
    }
}
