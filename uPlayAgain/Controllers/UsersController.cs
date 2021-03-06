﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using uPlayAgain.Data.Dto;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.Models;
using uPlayAgain.Utilities;

namespace uPlayAgain.Controllers
{
    public class UsersController : BaseController
    {
        private static readonly int PAGE_COUNT = 40;

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

            int libraryId = db.Libraries.Where(x => x.UserId == user.Id).FirstOrDefault().LibraryId;
            int gameInLibrary = db.LibraryComponents
                                  .Where(p => p.LibraryId == libraryId)
                                  .Where(p => p.IsDeleted == false)
                                  .Count();

            IList<Feedback> feedbacks = await db.Feedbacks.Where(p => p.UserId == id).ToListAsync();
            if (feedbacks == null)
            {
                return NotFound();
            }

            int feedbackCounter = feedbacks.Sum(p => p.Rate);
            double feedbackRate = default(double);

            if (feedbacks.Count == 0)
                feedbackRate = 0;
            else
                feedbackRate = 100 * (double)(feedbackCounter / feedbacks.Count);

            if (feedbackRate < 0) { feedbackRate = 0; }

            FeedbackRate fr = new FeedbackRate()
            {
                Rate = feedbackRate,
                Counter = feedbackCounter
            };

            UserResponse response = new UserResponse()
            {
                Id = user.Id,
                UserId = user.UserId,
                Username = user.UserName,
                Mail = user.Email,
                PositionUser = user.PositionUser,
                Image = user.Image,
                LastLogin = user.LastLogin,
                GameInLibrary = gameInLibrary,
                LibrariesId = new List<int>(libraryId),
                FeedbackAvg = (float)fr.Rate,
                FeedbackCount = fr.Counter
            };

            return Ok(response);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }
            
            User userToSave = await _userManager.FindByIdAsync(user.Id);
            if (string.Compare(userToSave.Email, user.Email) != 0 && ! string.IsNullOrEmpty(user.Email))
                await _userManager.SetEmailAsync(user.Id, user.Email);
            if(user.Image != null)
                userToSave.Image = user.Image;
            if(user.PositionUser != null)
                userToSave.PositionUser = user.PositionUser;
                        
            await _userManager.UpdateAsync(userToSave);                
            
            return Ok();            
        }
        
        [HttpGet]
        [AllowAnonymous]
        [Route("api/Users/Load/{id}")]
        public async Task<IHttpActionResult> Load(string id)
        {
            // Verifica mail abilitata
            User userLogin = await _userManager.FindByIdAsync(id);
            if (userLogin != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(userLogin.Id))
                {
                    return BadRequest("L'utente non ha ancora confermato l'indirizzo mail. Il login non può essere effettuato");
                }
            }

            if (userLogin.LastLogin == DateTimeOffset.MinValue || userLogin.LastLogin < DateTimeOffset.Now)
                userLogin.LastLogin = DateTimeOffset.Now;

            db.Entry(userLogin).State = EntityState.Modified;
            await db.SaveChangesAsync();

            UserResponse response = new UserResponse()
            {
                Id = userLogin.Id,
                UserId = userLogin.UserId,
                Username = userLogin.UserName,
                Mail = userLogin.Email,
                PositionUser = userLogin.PositionUser,
                Image = userLogin.Image,
                LibrariesId = await db.Libraries.Where(x => x.UserId == userLogin.Id).Select(x => x.LibraryId).ToListAsync()
            };

            return Ok(response);            
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
            IQueryable<Message> incoming = db.Messages.Where(m => m.UserReceiving_Id == id && !m.IsAlreadyReadReceiving && !m.IsAlreadyDeleteReceiving);

            IQueryable<Message> outgoing = db.Messages.Where(m => m.UserProponent_Id == id && !m.IsAlreadyReadProponent && !m.IsAlreadyDeleteProponent);

            int resultTran = await db.Transactions
                                     .Where(t => t.UserProponent_Id == id || t.UserReceiving_Id == id)
                                     .Where(t => t.Proposals.Count > 0)
                                     .OrderByDescending(t => t.Proposals.OrderByDescending(p => p.DateStart).FirstOrDefault().DateStart)
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
                                     .Where(
                                            x => (x.LastProposals.UserProponent_ProposalStatus == ProposalStatus.Accettata && x.LastProposals.UserReceiving_ProposalStatus == ProposalStatus.DaApprovare)
                                        )
                                     .CountAsync();

            return Ok(new MessageCountResponse
            {
                Incoming = await incoming.CountAsync(),
                Outgoing = await outgoing.CountAsync(),
                Transactions = resultTran
            });
        }
        
        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/incoming/{page}")]
        [ResponseType(typeof(Message))]
        public IQueryable<Message> GetIncomingMessages(string id, ushort page)
        {
            return db.Messages
                     .Where(m => m.UserReceiving_Id == id && !m.IsAlreadyDeleteReceiving)
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
                     .Where(m => m.UserProponent_Id == id && !m.IsAlreadyDeleteProponent)
                     .OrderByDescending(m => m.MessageDate)
                     .Skip((page - 1) * PAGE_COUNT)
                     .Take(PAGE_COUNT);
        }

        // GET: api/Messages/ByUser/5
        [Route("api/Messages/ByUser/{id}/transactions/{page}")]
        [ResponseType(typeof(List<TransactionDto>))]
        public async Task<IHttpActionResult> GetTransactionByUser(string id, ushort page)
        {
            List<TransactionDto> result = new List<TransactionDto>();            

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
                        LastProposalsNumber = x.Proposals.Count,
                        Components = x.Proposals
                                      .OrderByDescending(p => p.DateStart)
                                      .FirstOrDefault()
                                      .ProposalComponents
                                      .Where(y => y.Proposal.ProposalId == x.Proposals.OrderByDescending(p => p.DateStart).FirstOrDefault().ProposalId)
                                      .Select(z => new { LibraryComponents = z.LibraryComponents, UserId = z.LibraryComponents.Library.UserId })
                    })
                    // Mostro tutte le proposte in attesa di approvazione dall'altro utente e le proposta non annullate dall'utente corrente.
                    .Where(
                        x => (x.LastProposals.UserProponent_ProposalStatus == ProposalStatus.Accettata && x.LastProposals.UserReceiving_ProposalStatus == ProposalStatus.DaApprovare) ||
                        (x.LastProposals.UserProponent_ProposalStatus == ProposalStatus.DaApprovare && x.LastProposals.UserReceiving_ProposalStatus == ProposalStatus.Accettata)
                    )
                    .ToListAsync();
                     
            trans.ForEach(t =>
                    {
                        bool isProponent = (t.Transaction.UserProponent_Id == id) ;

                        List<LibraryComponent> myComponents = t.Components.Where(c => c.UserId == id).Select(x => x.LibraryComponents).ToList();
                        List<LibraryComponent> theirComponents = t.Components.Where(c => c.UserId != id).Select(x => x.LibraryComponents).ToList();
                    
                        result.Add(new TransactionDto()
                        {
                            TransactionId = t.Transaction.TransactionId,
                            TheirLibraryId = db.Libraries.Where(p => p.UserId == (isProponent ? t.Transaction.UserReceiving_Id : t.Transaction.UserProponent_Id)).First().LibraryId,
                            Proposal = t.LastProposals,
                            ProposalNumber = t.LastProposalsNumber,
                            LastChange = t.LastProposals.DateStart,
                            UserOwnerId = t.Transaction.UserProponent_Id,
                            Direction = t.LastProposals.Direction,
                            UserId = isProponent ? t.Transaction.UserReceiving_Id : t.Transaction.UserProponent_Id,
                            MyStatus = isProponent ? t.LastProposals.UserProponent_ProposalStatus : t.LastProposals.UserReceiving_ProposalStatus,
                            TheirStatus = isProponent ? t.LastProposals.UserReceiving_ProposalStatus : t.LastProposals.UserProponent_ProposalStatus,
                            MyItems = myComponents,
                            TheirItems = theirComponents
                        });
                    });

            return Ok(result);
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

        [Route("api/GameExchangeable/ByUser/{id}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetGamesCompleteByUser(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
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
                             if (lc.IsExchangeable)
                             {
                                components.Add(
                                    new LibraryComponentDto
                                 {
                                     GameLanguage = lc.GameLanguage,
                                     Games = new Game()
                                     {
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
                                     LibraryComponents = new LibraryComponent()
                                     {
                                         GameId = lc.GameId,
                                         GameLanguageId = lc.GameLanguageId,
                                         IsDeleted = lc.IsDeleted,
                                         IsExchangeable = lc.IsExchangeable,
                                         LibraryComponentId = lc.LibraryComponentId,
                                         LibraryId = lc.LibraryId,
                                         Note = lc.Note,
                                         StatusId = lc.StatusId
                                     },
                                     Status = lc.Status,
                                     UserId = lc.Library.UserId
                                 });
                            }
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