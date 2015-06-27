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
    public class GameLanguagesController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/GameLanguages
        public IQueryable<GameLanguage> GetGameLanguages()
        {
            return db.GameLanguages;
        }

        // GET: api/GameLanguages/5
        [ResponseType(typeof(GameLanguage))]
        public async Task<IHttpActionResult> GetGameLanguage(int id)
        {
            GameLanguage gameLanguage = await db.GameLanguages.FindAsync(id);
            if (gameLanguage == null)
            {
                return NotFound();
            }

            return Ok(gameLanguage);
        }

        // PUT: api/GameLanguages/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGameLanguage(int id, GameLanguage gameLanguage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != gameLanguage.GameLanguageId)
            {
                return BadRequest();
            }

            db.Entry(gameLanguage).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameLanguageExists(id))
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

        // POST: api/GameLanguages
        [ResponseType(typeof(GameLanguage))]
        public async Task<IHttpActionResult> PostGameLanguage(GameLanguage gameLanguage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GameLanguages.Add(gameLanguage);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = gameLanguage.GameLanguageId }, gameLanguage);
        }

        // DELETE: api/GameLanguages/5
        [ResponseType(typeof(GameLanguage))]
        public async Task<IHttpActionResult> DeleteGameLanguage(int id)
        {
            GameLanguage gameLanguage = await db.GameLanguages.FindAsync(id);
            if (gameLanguage == null)
            {
                return NotFound();
            }

            db.GameLanguages.Remove(gameLanguage);
            await db.SaveChangesAsync();

            return Ok(gameLanguage);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameLanguageExists(int id)
        {
            return db.GameLanguages.Count(e => e.GameLanguageId == id) > 0;
        }
    }
}