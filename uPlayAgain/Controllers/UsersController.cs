using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class UsersController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            User user = await Task.Run(() => db.Users.FirstOrDefault(u => u.UserId == id));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // GET: api/Users/5
        [Route("api/Users/Identity/{id}")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUserIdentity(string id)
        {
            User user = await Task.Run(() => db.Users.Find(id));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != user.UserId)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
                                                              .Where(y => y.Count > 0)
                                                              .ToList();

                return BadRequest(ModelState);
            }

            if (db.Users.Where(u => u.UserName.CompareTo(user.UserName) == 0 && u.Provider.CompareTo(user.Provider) == 0).Any())
            {
                return BadRequest("Utente già presente");                
            }
            else
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
            }            
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        #region ExtractByUser
        // GET: api/Messages/ByUser/5
        [Route("api/Transactions/ByUser/{id:int}")]
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> GetTransactionByUser(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            // Esplicito di caricare gli oggetti Game, ProposalComponent e Library Component!
            await db.Proposals.LoadAsync();
            await db.ProposalComponents.LoadAsync();
            await db.LibraryComponents.LoadAsync();
            // Esplicito di caricare gli oggetti Game!
            await db.Games.LoadAsync();
            await db.Genres.LoadAsync();
            await db.Platforms.LoadAsync();
            await db.GameLanguages.LoadAsync();
            await db.Status.LoadAsync();

            List<User> users = await db.Users
                                       .Include(t => t.TransactionsProponent)
                                       .Include(t => t.TransactionsReceiving)
                                       .Where(t => t.UserId == id)
                                       .ToListAsync();
            return Ok(users);
        }


        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id:int}")]
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> GetMessage(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            List<User> users = await db.Users
                                       .Include(t => t.MessagesIn)
                                       .Include(t => t.MessagesIn.Select(p => p.UserProponent))
                                       .Include(t => t.MessagesIn.Select(p => p.UserReceiving))
                                       .Include(t => t.MessagesOut)
                                       .Include(t => t.MessagesOut.Select(p => p.UserProponent))
                                       .Include(t => t.MessagesOut.Select(p => p.UserReceiving))
                                       .Where(t => t.UserId == id)
                                       .ToListAsync();

            return Ok(users);
        }

        // GET: api/Libraries/ByUser/5
        [Route("api/Libraries/ByUser/{id:int}")]
        [ResponseType(typeof(Library))]
        public async Task<IHttpActionResult> GetLibraryByUser(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            // Esplicito di caricare gli oggetti Game!
            db.Games.Load();
            db.Genres.Load();
            db.Platforms.Load();
            db.GameLanguages.Load();
            db.Status.Load();

            List<User> users = await db.Users
                                       .Include(t => t.Libraries)
                                       .Include(t => t.Libraries.Select(p => p.User))
                                       .Include(t => t.Libraries.Select(p => p.LibraryComponents))
                                       .Where(t => t.UserId == id)
                                       .ToListAsync();
            return Ok(users);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserId == id) > 0;
        }
    }
}