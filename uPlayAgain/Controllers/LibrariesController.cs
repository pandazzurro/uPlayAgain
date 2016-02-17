using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Controllers
{
    public class LibrariesController : BaseController
    {
        // GET: api/Libraries
        public IQueryable<Library> GetLibrarys()
        {
            return db.Libraries;
        }

        // GET: api/Libraries/5
        [ResponseType(typeof(Library))]
        public async Task<IHttpActionResult> GetLibrary(int id)
        {
            if (await db.Libraries.FindAsync(id) == null)
            {
                return NotFound();
            }
            
            return Ok(
                await db.Libraries
                        .Include(t => t.LibraryComponents)
                        .Include(t => t.User)
                        .Where(t => t.LibraryId == id)
                        .Select(x => new
                        {
                            LibraryComponents = x.LibraryComponents,
                            LibraryId = x.LibraryId,
                            User = x.User,
                            UserId = x.UserId
                        })
                        .SingleOrDefaultAsync()
                    );
        }

        // PUT: api/Libraries/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLibrary(int id, Library library)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != library.LibraryId)
            {
                return BadRequest();
            }

            db.Entry(library).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibraryExists(id))
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

        // POST: api/Libraries
        [ResponseType(typeof(Library))]
        public async Task<IHttpActionResult> PostLibrary(Library library)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Libraries.Add(library);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = library.LibraryId }, library);
        }

        // DELETE: api/Libraries/5
        [ResponseType(typeof(Library))]
        public async Task<IHttpActionResult> DeleteLibrary(int id)
        {
            Library library = await db.Libraries.FindAsync(id);
            if (library == null)
            {
                return NotFound();
            }

            db.Libraries.Remove(library);
            await db.SaveChangesAsync();

            return Ok(library);
        }
        
        private bool LibraryExists(int id)
        {
            return db.Libraries.Count(e => e.LibraryId == id) > 0;
        }
    }
}