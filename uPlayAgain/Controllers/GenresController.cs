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
    public class GenresController : BaseController
    {
        // GET: api/Genres
        public IQueryable<Genre> GetGenres()
        {
            return db.Genres;
        }

        // GET: api/Genres/5
        [ResponseType(typeof(Genre))]
        public async Task<IHttpActionResult> GetGenre(string id)
        {
            Genre genre = await db.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        // PUT: api/Genres/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGenre(string id, Genre genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != genre.Name)
            {
                return BadRequest();
            }

            db.Entry(genre).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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

        // POST: api/Genres
        [ResponseType(typeof(Genre))]
        public async Task<IHttpActionResult> PostGenre(Genre genre)
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
                if (GenreExists(genre.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = genre.Name }, genre);
        }

        // DELETE: api/Genres/5
        [ResponseType(typeof(Genre))]
        public async Task<IHttpActionResult> DeleteGenre(string id)
        {
            Genre genre = await db.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            db.Genres.Remove(genre);
            await db.SaveChangesAsync();

            return Ok(genre);
        }
        
        private bool GenreExists(string id)
        {
            return db.Genres.Count(e => e.Name == id) > 0;
        }
    }
}