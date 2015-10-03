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
        public class UserResponse
        {
            public string Id;
            public string Username;
            public float FeedbackAvg;
            public int FeedbackCount;
        }

        public class MessageCountResponse
        {
            public int Incoming;
            public int Outgoing;
            public int Transactions;
        }

        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(UserResponse))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            User user = await Task.Run(() => db.Users.FirstOrDefault(u => u.UserId == id));
            if (user == null)
            {
                return NotFound();
            }

            IQueryable<Feedback> feedbacks = await Task.Run(() => db.Feedbacks.Where(f => f.UserId == user.Id));

            UserResponse result = new UserResponse()
            {
                Id = user.Id,
                Username = user.UserName,
                FeedbackAvg = (float)feedbacks.Average(f => (float)f.Rate),
                FeedbackCount = feedbacks.Count()
            };

            return Ok(result);
        }


        // GET: api/Users/5
        [Route("api/Users/Identity/{id}")]
        [ResponseType(typeof(UserResponse))]
        public async Task<IHttpActionResult> GetUserIdentity(string id)
        {
            User user = await Task.Run(() => db.Users.Find(id));
            if (user == null)
            {
                return NotFound();
            }

            return await GetUser(user.UserId);
        }


        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }
            else
            {
                User userToSave = await db.Users.Where(p => p.UserId == id).FirstOrDefaultAsync();
                userToSave.Image = user.Image;
                userToSave.Email = user.Email;
                userToSave.PasswordHash = user.PasswordHash;
                userToSave.PositionUser = user.PositionUser;
                userToSave.Provider = user.Provider;
                userToSave.UserName = user.UserName;
                db.Entry(userToSave).State = EntityState.Modified;
            }
            
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
        //[ResponseType(typeof(User))]
        //public async Task<IHttpActionResult> PostUser(User user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
        //                                                      .Where(y => y.Count > 0)
        //                                                      .ToList();

        //        return BadRequest(ModelState);
        //    }

        //    if (db.Users.Where(u => u.UserName.CompareTo(user.UserName) == 0 && u.Provider.CompareTo(user.Provider) == 0).Any())
        //    {
        //        return BadRequest("Utente già presente");                
        //    }
        //    else
        //    {
        //        db.Users.Add(user);
        //        await db.SaveChangesAsync();
        //        return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        //    }            
        //}

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

        #region CheckUser
        [Route("api/Users/Exists/{username}")]
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> CheckByUsername(string username)
        {
            bool exist = await db.Users.Where(t => string.Compare(t.UserName,username, true) == 0).AnyAsync();
            if (!exist)
                return NotFound();
            return Ok();
        }
        #endregion

        #region ExtractByUser
        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/transactions/{page}")]
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> GetTransactionByUser(string id, ushort page)
        {
            IQueryable<Transaction> transactions = await Task.Run(() => db.Transactions
                .Where(t => t.UserProponent_Id == id || t.UserReceiving_Id == id));

            transactions = await Task.Run(() => transactions.Where(t => t.Proposals.Count > 0)
                .OrderByDescending(t => t.Proposals.OrderByDescending(p => p.DateStart).FirstOrDefault().DateStart)
                .Skip((page - 1) * PAGE_COUNT)
                .Take(PAGE_COUNT));

            return Ok(transactions.ToListAsync());
            /*
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
            return Ok(users);*/
        }


        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}")]
        [ResponseType(typeof(MessageCountResponse))]
        public async Task<IHttpActionResult> GetMessages(string id)
        {
            /*
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }*/
            
            IQueryable<Message> incoming = await Task.Run(() => db.Messages
                .Where(m => m.UserReceiving_Id == id && !m.IsAlreadyRead));

            IQueryable<Message> outgoing = await Task.Run(() => db.Messages
                .Where(m => m.UserProponent_Id == id && !m.IsAlreadyRead));

            IQueryable<Transaction> transactions = await Task.Run(() =>  db.Transactions
                .Where(t => t.UserProponent_Id == id || t.UserReceiving_Id == id)
                .Where(t => t.Proposals.Count > 0)
                .Where(t => t.TransactionStatus != TransactionStatus.Conclusa));

            MessageCountResponse result = new MessageCountResponse()
            {
                Incoming = incoming.Count(),
                Outgoing = outgoing.Count(),
                Transactions = transactions.Count()
            };
            /*
            List<User> users = await db.Users
                                       .Include(t => t.MessagesIn)
                                       .Include(t => t.MessagesIn.Select(p => p.UserProponent))
                                       .Include(t => t.MessagesIn.Select(p => p.UserReceiving))
                                       .Include(t => t.MessagesOut)
                                       .Include(t => t.MessagesOut.Select(p => p.UserProponent))
                                       .Include(t => t.MessagesOut.Select(p => p.UserReceiving))
                                       .Where(t => t.UserId == id)
                                       .ToListAsync();
            */
            return Ok(result);
        }

        private static readonly int PAGE_COUNT = 40;

        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/incoming/{page}")]
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> GetIncomingMessages(string id, ushort page)
        {
            IQueryable<Message> incoming = await Task.Run(() => db.Messages
                .Where(m => m.UserReceiving_Id == id)
                .OrderByDescending(m => m.MessageDate)
                .Skip((page - 1) * PAGE_COUNT)
                .Take(PAGE_COUNT));

            return Ok(incoming.ToListAsync());
        }

        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/outgoing/{page}")]
        [ResponseType(typeof(Message))]
        public async Task<IHttpActionResult> GetOutgoingMessages(string id, ushort page)
        {
            IQueryable<Message> outgoing = await Task.Run(() => db.Messages
                .Where(m => m.UserProponent_Id == id)
                .OrderByDescending(m => m.MessageDate)
                .Skip((page - 1) * PAGE_COUNT)
                .Take(PAGE_COUNT));

            return Ok(outgoing.ToListAsync());
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

        [Route("api/Feedbacks/ByUser/{id:int}")]
        [ResponseType(typeof(Library))]
        public async Task<IHttpActionResult> GetFeedbacksByUser(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            
            // Esplicito di caricare gli oggetti presenti nelle transazioni
            db.Games.Load();
            db.Genres.Load();
            db.Platforms.Load();
            db.GameLanguages.Load();
            db.Status.Load();
            db.Proposals.Load();
            db.ProposalComponents.Load();
            db.Libraries.Load();
            db.LibraryComponents.Load();

            List<User> users = await db.Users
                                       .Include(t => t.Feedbacks)
                                       .Include(t => t.Feedbacks.Select(p => p.User))
                                       .Include(t => t.Feedbacks.Select(p => p.Transaction))
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