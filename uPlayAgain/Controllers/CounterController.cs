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
    public class CounterController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Counter/GamesByUser/5
        [Route("api/Counter/GamesByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetGamesByUser(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            int result = await db.Libraries
                                 .Include(t => t.LibraryComponents)
                                 .Include(t => t.User)
                                 .Where(t => t.User.UserId == id)
                                 .CountAsync();
            return Ok(result);
        }

        // GET: api/Counter/GamesByUser/5
        [Route("api/Counter/LibrariesByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetLibrariesByUser(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            int result = await db.Libraries
                                 .Include(t => t.User)
                                 .Where(t => t.User.UserId == id)
                                 .CountAsync();
            return Ok(result);
        }

        // GET: api/Counter/GamesByUser/5
        [Route("api/Counter/MessagesByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetMessagesByUser(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            int resultMessage = await db.Messages
                                     .Include(t => t.UserReceiving)
                                     .Where(t => t.UserReceiving.UserId == id && t.IsAlreadyReadReceiving == false)
                                     .CountAsync();
                        
            int resultTran = await db.Transactions
                   .Where(t => t.UserProponent_Id == user.Id || t.UserReceiving_Id == user.Id)
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
                   // Mostro tutte le proposte approvate da entrambi gli utenti e senza feedback da parte dell'utente corrente
                   .Where(
                       x => (x.LastProposals.UserProponent_ProposalStatus == ProposalStatus.Accettata && x.LastProposals.UserReceiving_ProposalStatus == ProposalStatus.Accettata)
                            && !x.Transaction.Feedbacks.Where(y => y.UserId != user.Id).Any() //Feedback rilasciati DA me!
                   )
                   .CountAsync();

            return Ok(resultMessage + resultTran);
        }

        //// GET: api/Counter/TransactionsIngoingByUser/5
        //[Route("api/Counter/TransactionsIngoingByUser/{id:int}")]
        //[ResponseType(typeof(int))]
        //public async Task<IHttpActionResult> GetTransactionsIngoingByUser(int id)
        //{
        //    User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    int result = await db.Transactions
        //                         .Include(t => t.UserReceiving)
        //                         .Include(t => t.Proposals)
        //                         .Where(t => t.UserReceiving.UserId == id)
        //                         .Where(t => t.TransactionStatus != TransactionStatus.Conclusa)
        //                         .CountAsync();
        //    return Ok(result);
        //}

        //// GET: api/Counter/TransactionsOutgoingByUser/5
        //[Route("api/Counter/TransactionsOutgoingByUser/{id:int}")]
        //[ResponseType(typeof(int))]
        //public async Task<IHttpActionResult> GetTransactionsOutgoingByUser(int id)
        //{
        //    User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    int result = await db.Transactions
        //                         .Include(t => t.UserReceiving)
        //                         .Include(t => t.Proposals)
        //                         .Where(t => t.UserProponent.UserId == id)
        //                         .Where(t => t.TransactionStatus != TransactionStatus.Conclusa)
        //                         .CountAsync();
        //    return Ok(result);
        //}
        
        // GET: api/Counter/ByUser/''
        [Route("api/Counter/ByUser/{id}")]
        [ResponseType(typeof(MessageCountResponse))]

        public async Task<IHttpActionResult> GetCounter(string id)
        {
            IQueryable<Message> incoming = db.Messages.Where(m => m.UserReceiving_Id == id && !m.IsAlreadyReadReceiving);

            IQueryable<Message> outgoing = db.Messages.Where(m => m.UserProponent_Id == id && !m.IsAlreadyReadProponent);

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

            IQueryable<LibraryComponent> libraryComponents = db.LibraryComponents.Include(z => z.Library).Where(m => m.Library.UserId == id && m.IsDeleted == false);

            return Ok(new MessageCountResponse
            {
                Incoming = await incoming.CountAsync(),
                Outgoing = await outgoing.CountAsync(),
                Transactions = resultTran,
                LibrariesComponents = await libraryComponents.CountAsync()
            });
        }

        // GET: api/Counter/GamesByUser/5
        [Route("api/Counter/FeedbackByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetRatingByUser(int id)
        {
            User user = await db.Users.Where(t => t.UserId == id).SingleOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            int rateCount = await db.Feedbacks
                                  .Include(t => t.User)
                                  .Where(t => t.User.UserId == id)
                                  .CountAsync();

            int rateSum = await db.Feedbacks
                                  .Include(t => t.User)
                                  .Where(t => t.User.UserId == id)
                                  .SumAsync(t => t.Rate);

            int result = 100 * (rateSum / rateCount);
            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
    }
}