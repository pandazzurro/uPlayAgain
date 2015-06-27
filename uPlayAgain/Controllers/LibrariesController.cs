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
using System.Web.Http.Description;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class LibrariesController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Libraries
        public IQueryable<Library> GetLibrarys()
        {
            return db.Librarys;
        }

        // GET: api/Libraries/5
        [ResponseType(typeof(Library))]
        public async Task<IHttpActionResult> GetLibrary(int id)
        {
            Library library = await db.Librarys.FindAsync(id);
            if (library == null)
            {
                return NotFound();
            }

            return Ok(library);
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

            db.Librarys.Add(library);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = library.LibraryId }, library);
        }

        // DELETE: api/Libraries/5
        [ResponseType(typeof(Library))]
        public async Task<IHttpActionResult> DeleteLibrary(int id)
        {
            Library library = await db.Librarys.FindAsync(id);
            if (library == null)
            {
                return NotFound();
            }

            db.Librarys.Remove(library);
            await db.SaveChangesAsync();

            return Ok(library);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LibraryExists(int id)
        {
            return db.Librarys.Count(e => e.LibraryId == id) > 0;
        }
    }
}