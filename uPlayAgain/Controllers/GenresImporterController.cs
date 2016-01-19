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
using System.Web.ModelBinding;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using uPlayAgain.Data.EF.Context;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.OData.Builder;
    using System.Web.OData.Extensions;
    using uPlayAgain.Data.EF.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Genre>("GenresImporter");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class GenresImporterController : ODataController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: odata/GenresImporter
        [EnableQuery]
        public IQueryable<Genre> GetGenresImporter()
        {
            return db.Genres;
        }

        // GET: odata/GenresImporter(5)
        [EnableQuery]
        public SingleResult<Genre> GetGenre([FromODataUri] string key)
        {
            return SingleResult.Create(db.Genres.Where(genre => genre.GenreId == key));
        }

        // PUT: odata/GenresImporter(5)
        public async Task<IHttpActionResult> Put([FromODataUri] string key, Delta<Genre> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Genre genre = await db.Genres.FindAsync(key);
            if (genre == null)
            {
                return NotFound();
            }

            patch.Put(genre);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(genre);
        }

        // POST: odata/GenresImporter
        public async Task<IHttpActionResult> Post(Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Genres.Add(genre);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GenreExists(genre.GenreId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(genre);
        }

        // PATCH: odata/GenresImporter(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] string key, Delta<Genre> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Genre genre = await db.Genres.FindAsync(key);
            if (genre == null)
            {
                return NotFound();
            }

            patch.Patch(genre);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(genre);
        }

        // DELETE: odata/GenresImporter(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] string key)
        {
            Genre genre = await db.Genres.FindAsync(key);
            if (genre == null)
            {
                return NotFound();
            }

            db.Genres.Remove(genre);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GenreExists(string key)
        {
            return db.Genres.Count(e => e.GenreId == key) > 0;
        }
    }
}
