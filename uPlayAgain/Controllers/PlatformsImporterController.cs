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
    builder.EntitySet<Platform>("PlatformsImporter");
    config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class PlatformsImporterController : ODataController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: odata/PlatformsImporter
        [EnableQuery]
        public IQueryable<Platform> GetPlatformsImporter()
        {
            return db.Platforms;
        }

        // GET: odata/PlatformsImporter(5)
        [EnableQuery]
        public SingleResult<Platform> GetPlatform([FromODataUri] string key)
        {
            return SingleResult.Create(db.Platforms.Where(platform => platform.PlatformId == key));
        }

        // PUT: odata/PlatformsImporter(5)
        public async Task<IHttpActionResult> Put([FromODataUri] string key, Delta<Platform> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Platform platform = await db.Platforms.FindAsync(key);
            if (platform == null)
            {
                return NotFound();
            }

            patch.Put(platform);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(platform);
        }

        // POST: odata/PlatformsImporter
        public async Task<IHttpActionResult> Post(Platform platform)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Platforms.Add(platform);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PlatformExists(platform.PlatformId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(platform);
        }

        // PATCH: odata/PlatformsImporter(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] string key, Delta<Platform> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Platform platform = await db.Platforms.FindAsync(key);
            if (platform == null)
            {
                return NotFound();
            }

            patch.Patch(platform);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(platform);
        }

        // DELETE: odata/PlatformsImporter(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] string key)
        {
            Platform platform = await db.Platforms.FindAsync(key);
            if (platform == null)
            {
                return NotFound();
            }

            db.Platforms.Remove(platform);
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

        private bool PlatformExists(string key)
        {
            return db.Platforms.Count(e => e.PlatformId == key) > 0;
        }
    }
}
