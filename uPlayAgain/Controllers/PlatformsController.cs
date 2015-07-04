using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class PlatformsController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Platforms
        public IQueryable<Platform> GetPlatforms()
        {
            return db.Platforms;
        }

        // GET: api/Platforms/5
        [ResponseType(typeof(Platform))]
        public async Task<IHttpActionResult> GetPlatform(string id)
        {
            Platform platform = await db.Platforms.FindAsync(id);
            if (platform == null)
            {
                return NotFound();
            }

            return Ok(platform);
        }

        // PUT: api/Platforms/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPlatform(string id, Platform platform)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != platform.Name)
            {
                return BadRequest();
            }

            db.Entry(platform).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlatformExists(id))
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

        // POST: api/Platforms
        [ResponseType(typeof(Platform))]
        public async Task<IHttpActionResult> PostPlatform(Platform platform)
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
                if (PlatformExists(platform.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = platform.Name }, platform);
        }

        // DELETE: api/Platforms/5
        [ResponseType(typeof(Platform))]
        public async Task<IHttpActionResult> DeletePlatform(string id)
        {
            Platform platform = await db.Platforms.FindAsync(id);
            if (platform == null)
            {
                return NotFound();
            }

            db.Platforms.Remove(platform);
            await db.SaveChangesAsync();

            return Ok(platform);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlatformExists(string id)
        {
            return db.Platforms.Count(e => e.Name == id) > 0;
        }
    }
}