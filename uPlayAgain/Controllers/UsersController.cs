using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using uPlayAgain.Dto;
using uPlayAgain.Models;
using uPlayAgain.Utilities;

namespace uPlayAgain.Controllers
{
    public class UsersController : ApiController
    {
        private uPlayAgainContext db;
        private ApplicationUserManager _userManager;
        private static readonly int PAGE_COUNT = 40;

        public UsersController()
        {
            db = new uPlayAgainContext();
            _userManager = new ApplicationUserManager(new UserStore<User>(db));
        }
        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(UserResponse))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            UserResponse response = null;
            response = db.Feedbacks
                         .Where(f => f.UserId == user.Id)
                         .GroupBy(t => t.UserId, (key, g) => new { UserId = key, Feedback = g.ToList() })
                         .Select(t => new UserResponse()
                         {
                             Id = user.Id,
                             Username = user.UserName,
                             Mail = user.Email,
                             PositionUser = user.PositionUser,
                             Image = user.Image,
                             FeedbackAvg = t.Feedback.Average(f => (float)f.Rate),
                             FeedbackCount = t.Feedback.Count()
                         })
                         .FirstOrDefault();
            if(response == null)
            {
                response = new UserResponse()
                {
                    Id = user.Id,
                    UserId = user.UserId,
                    Username = user.UserName,
                    Mail = user.Email,                    
                    PositionUser = user.PositionUser,
                    Image = user.Image
                };
            }

            return Ok(response);
        }


        // GET: api/Users/5
        [Route("api/Users/Identity/{id}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetUserIdentity(string id)
        {
            User user = await Task.Run(() => db.Users.Find(id));
            if (user == null)
            {
                return NotFound();
            }

            return await GetUser(user.UserId);
        }

        // GET: api/Users/5
        [Route("api/Users/Profile/{id}")]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUserProfile(string id)
        {
            User user = await Task.Run(() => db.Users.FirstOrDefault(u => u.Id == id));
            if (user == null)
            {
                return NotFound();
            }
            
            int gameInLibrary = db.LibraryComponents.Where(p => p.LibraryId == db.Libraries.Where(x => x.UserId == user.Id).FirstOrDefault().LibraryId).Count();

            UserResponse response = new UserResponse()
            {
                Id = user.Id,
                UserId = user.UserId,
                Username = user.UserName,
                Mail = user.Email,
                PositionUser = user.PositionUser,
                Image = user.Image,
                LastLogin = user.LastLogin,
                GameInLibrary = gameInLibrary
            };

            return Ok(response);
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
            db.Libraries.Where(t => t.UserId == user.Id).ToList().ForEach(lib =>
            {
                db.Libraries.Remove(lib);
            });            
            await db.SaveChangesAsync();

            return Ok(user);
        }

        #region CheckUser
        [Route("api/Users/ExistsUser/{username}")]
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> CheckByUsername(string username)
        {
            if (await _userManager.FindByNameAsync(username) == null)
                return NotFound();
            return Ok();
        }

        [Route("api/Users/ExistsMail/{username}")]
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> CheckByMail(string mail)
        {
            if (await _userManager.FindByEmailAsync(mail) == null)
                return NotFound();
            return Ok();
        }
        #endregion

        #region ExtractByUser
        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}")]
        [ResponseType(typeof(MessageCountResponse))]
        public async Task<IHttpActionResult> GetMessages(string id)
        {
            IQueryable<Message> incoming = db.Messages.Where(m => m.UserReceiving_Id == id && !m.IsAlreadyRead);

            IQueryable<Message> outgoing = db.Messages.Where(m => m.UserProponent_Id == id && !m.IsAlreadyRead);

            IQueryable<Transaction> transactions = db.Transactions
                                                     .Where(t => t.UserProponent_Id == id || t.UserReceiving_Id == id)
                                                     .Where(t => t.Proposals.Count > 0)
                                                     .Where(t => t.TransactionStatus != TransactionStatus.Conclusa);

            return Ok(new MessageCountResponse
            {
                Incoming = await incoming.CountAsync(),
                Outgoing = await outgoing.CountAsync(),
                Transactions = await transactions.CountAsync()
            });
        }
        
        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/incoming/{page}")]
        [ResponseType(typeof(Message))]
        public IQueryable<Message> GetIncomingMessages(string id, ushort page)
        {
            return db.Messages
                     .Where(m => m.UserReceiving_Id == id)
                     .OrderByDescending(m => m.MessageDate)
                     .Skip((page - 1) * PAGE_COUNT)
                     .Take(PAGE_COUNT);
        }

        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/outgoing/{page}")]
        [ResponseType(typeof(Message))]
        public IQueryable<Message> GetOutgoingMessages(string id, ushort page)
        {
            return db.Messages
                     .Where(m => m.UserProponent_Id == id)
                     .OrderByDescending(m => m.MessageDate)
                     .Skip((page - 1) * PAGE_COUNT)
                     .Take(PAGE_COUNT);
        }

        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/transactions/{page}")]
        [ResponseType(typeof(TransactionDto))]
        public async Task<IList<TransactionDto>> GetTransactionByUser(string id, ushort page)
        {
            IList<TransactionDto> result = new List<TransactionDto>();

            var trans = await db.Transactions
                    .Where(t => t.UserProponent_Id == id || t.UserReceiving_Id == id)
                    .Where(t => t.Proposals.Count > 0)
                    .OrderByDescending(t => t.Proposals.OrderByDescending(p => p.DateStart).FirstOrDefault().DateStart)
                    .Skip((page - 1) * PAGE_COUNT)
                    .Take(PAGE_COUNT)
                    .Select(x => new
                    {
                        Transaction = x,
                        LastProposals = x.Proposals
                                         .OrderByDescending(p => p.DateStart)
                                         .FirstOrDefault(),
                        Components = x.Proposals
                                      .OrderByDescending(p => p.DateStart)
                                      .FirstOrDefault()
                                      .ProposalComponents
                                      .Where(y => y.Proposal.ProposalId == x.Proposals.OrderByDescending(p => p.DateStart).FirstOrDefault().ProposalId)
                                      .Select(z => new { LibraryComponents = z.LibraryComponents, UserId = z.LibraryComponents.Library.UserId })
                    })           
                    .ToListAsync();
                     
            trans.ForEach(t =>
                    {
                        bool isProponent = (t.Transaction.UserProponent_Id == id);

                        List<LibraryComponent> myComponents = t.Components.Where(c => c.UserId == id).Select(x => x.LibraryComponents).ToList();
                        List<LibraryComponent> theirComponents = t.Components.Where(c => c.UserId != id).Select(x => x.LibraryComponents).ToList();
                    
                        result.Add(new TransactionDto()
                        {
                            LastChange = t.LastProposals.DateStart,
                            UserId = isProponent ? t.Transaction.UserReceiving_Id : t.Transaction.UserProponent_Id,
                            MyStatus = isProponent ? t.LastProposals.UserProponent_ProposalStatus : t.LastProposals.UserReceiving_ProposalStatus,
                            TheirStatus = isProponent ? t.LastProposals.UserReceiving_ProposalStatus : t.LastProposals.UserProponent_ProposalStatus,
                            MyItems = myComponents,
                            TheirItems = theirComponents
                        });
                    });
            
            return result;
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

        [Route("api/Games/ByUser/{id}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetGamesByUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            IList<int> games = new List<int>();

            await db.Libraries
                    .Include(x => x.LibraryComponents)
                    .Where(x => x.UserId == user.Id)
                    .ForEachAsync( l =>
                    {
                        l.LibraryComponents
                         .Select(lc => lc.GameId)
                         .ToList()
                         .ForEach(g => games.Add(g));
                    }); 
            
            return Ok(games);
        }

        [Route("api/GamesComplete/ByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetGamesCompleteByUser(int id)
        {
            User user = db.Users.Where(t => t.UserId == id).SingleOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            IList<LibraryComponentDto> components = new List<LibraryComponentDto>();

            await db.Libraries
                    .Include(x => x.LibraryComponents)
                    .Include(x => x.LibraryComponents.Select(z => z.Status))
                    .Include(x => x.LibraryComponents.Select(z => z.GameLanguage))
                    .Include(x => x.LibraryComponents.Select(z => z.Games))
                    .Where(x => x.UserId == user.Id)
                    .ForEachAsync(l =>
                    {
                        l.LibraryComponents
                         .ToList()
                         .ForEach(lc =>
                         {
                             components.Add(
                             new LibraryComponentDto
                             {
                                 GameLanguage = lc.GameLanguage,
                                 Games = new Game() {
                                     GameId = lc.Games.GameId,
                                     GenreId = lc.Games.GenreId,
                                     Genre = lc.Games.Genre,
                                     Platform = lc.Games.Platform,
                                     PlatformId = lc.Games.PlatformId,
                                     ShortName = lc.Games.ShortName,
                                     Title = lc.Games.Title,
                                     Description = lc.Games.Description,
                                     Image = null
                                 },
                                 LibraryComponents = new LibraryComponent() {
                                     GameId = lc.GameId,
                                     GameLanguageId = lc.GameLanguageId,
                                     IsDeleted = lc.IsDeleted,
                                     IsExchangeable = lc.IsExchangeable,
                                     LibraryComponentId = lc.LibraryComponentId,
                                     LibraryId = lc.LibraryId,
                                     Note = lc.Note,
                                     StatusId = lc.StatusId
                                 },
                                 Status = lc.Status
                             });
                         });                           
                    });

            return Ok(components);
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