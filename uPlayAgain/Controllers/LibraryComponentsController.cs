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
    public class LibraryComponentsController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/LibraryComponents
        public IQueryable<LibraryComponent> GetLibraryComponents()
        {
            return db.LibraryComponents;
        }

        // GET: api/LibraryComponents/5
        [ResponseType(typeof(LibraryComponent))]
        public async Task<IHttpActionResult> GetLibraryComponent(int id)
        {
            LibraryComponent libraryComponent = await db.LibraryComponents.FindAsync(id);
            if (libraryComponent == null)
            {
                return NotFound();
            }

            return Ok(libraryComponent);
        }

        // PUT: api/LibraryComponents/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLibraryComponent(int id, LibraryComponent libraryComponent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != libraryComponent.LibraryComponentId)
            {
                return BadRequest();
            }

            db.Entry(libraryComponent).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibraryComponentExists(id))
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

        // POST: api/LibraryComponents
        [ResponseType(typeof(LibraryComponent))]
        public async Task<IHttpActionResult> PostLibraryComponent(LibraryComponent libraryComponent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LibraryComponents.Add(libraryComponent);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LibraryComponentExists(libraryComponent.LibraryComponentId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = libraryComponent.LibraryComponentId }, libraryComponent);
        }

        // DELETE: api/LibraryComponents/5
        [ResponseType(typeof(LibraryComponent))]
        public async Task<IHttpActionResult> DeleteLibraryComponent(int id)
        {
            LibraryComponent libraryComponent = await db.LibraryComponents.FindAsync(id);
            if (libraryComponent == null)
            {
                return NotFound();
            }

            db.LibraryComponents.Remove(libraryComponent);
            await db.SaveChangesAsync();

            return Ok(libraryComponent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LibraryComponentExists(int id)
        {
            return db.LibraryComponents.Count(e => e.LibraryComponentId == id) > 0;
        }
    }
}