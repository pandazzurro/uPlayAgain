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

namespace uPlayAgain.Controllers
{
    public class FeedbacksController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Feedbacks
        public IQueryable<Feedback> GetFeedbacks()
        {
            return db.Feedbacks;
        }

        // GET: api/Feedbacks/5
        [ResponseType(typeof(Feedback))]
        public async Task<IHttpActionResult> GetFeedback(int id)
        {
            Feedback feedback = await db.Feedbacks.Where(p => p.TransactionId == id).FirstOrDefaultAsync();
            if (feedback == null)
            {
                return NotFound();
            }

            return Ok(feedback);
        }

        // GET: api/Feedbacks/5
        [Route("api/Feedbacks/Rate/{userId}")]        
        [ResponseType(typeof(FeedbackRate))]
        public async Task<IHttpActionResult> GetFeedbacksRate(string userId)
        {
            IList<Feedback> feedbacks = await db.Feedbacks.Where(p => p.UserId == userId).ToListAsync();
            if (feedbacks == null)
            {
                return NotFound();
            }

            int feedbackCounter = feedbacks.Sum(p => p.Rate);
            double feedbackRate = 100 * (double)(feedbackCounter / feedbacks.Count);
            if(feedbackRate < 0) { feedbackRate = 0; }

            FeedbackRate fr = new FeedbackRate()
            {
                Rate = feedbackRate,
                Counter = feedbackCounter
            };

            return Ok(fr);
        }

        // GET: api/Feedbacks/5
        [Route("api/Feedbacks/Pending/{id}")]
        [ResponseType(typeof(List<TransactionDto>))]
        public async Task<IHttpActionResult> GetTransactionFeedbacksPendings(string id)
        {
            List<TransactionDto> result = new List<TransactionDto>();

            var trans = await db.Transactions
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
                    // Mostro tutte le proposte approvate da entrambi gli utenti e senza feedback da parte dell'utente corrente
                    .Where(
                        x => (x.LastProposals.UserProponent_ProposalStatus == ProposalStatus.Accettata && x.LastProposals.UserReceiving_ProposalStatus == ProposalStatus.Accettata) 
                             && !x.Transaction.Feedbacks.Where(y => y.UserId != id).Any() //Feedback rilasciati DA me!
                    )
                    .ToListAsync();

            trans.ForEach(t =>
            {
                bool isProponent = (t.Transaction.UserProponent_Id == id);

                List<LibraryComponent> myComponents = t.Components.Where(c => c.UserId == id).Select(x => x.LibraryComponents).ToList();
                List<LibraryComponent> theirComponents = t.Components.Where(c => c.UserId != id).Select(x => x.LibraryComponents).ToList();

                result.Add(new TransactionDto()
                {
                    TransactionId = t.Transaction.TransactionId,
                    Proposal = t.LastProposals,
                    LastChange = t.LastProposals.DateStart,
                    UserOwnerId = t.Transaction.UserProponent_Id,
                    UserId = isProponent ? t.Transaction.UserReceiving_Id : t.Transaction.UserProponent_Id,
                    MyStatus = isProponent ? t.LastProposals.UserProponent_ProposalStatus : t.LastProposals.UserReceiving_ProposalStatus,
                    TheirStatus = isProponent ? t.LastProposals.UserReceiving_ProposalStatus : t.LastProposals.UserProponent_ProposalStatus,
                    MyItems = myComponents,
                    TheirItems = theirComponents
                });
            });

            return Ok(result);
        }


        // PUT: api/Feedbacks/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFeedback(int id, Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feedback.FeedbackId)
            {
                return BadRequest();
            }

            // Prima di inserire un feedback controllo se l'utente presente nel feedback è oggetto di transazione
            Transaction tran = await db.Transactions.Where(p => p.TransactionId == feedback.TransactionId).FirstOrDefaultAsync();
            if (tran.UserProponent_Id == feedback.UserId || tran.UserReceiving_Id == feedback.UserId)
            {
                db.Entry(feedback).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                BadRequest("Non è possibile assegnare un feedback ad un utente che non ha partecipato alla transazione");
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Feedbacks
        [ResponseType(typeof(Feedback))]
        public async Task<IHttpActionResult> PostFeedback(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Prima di inserire un feedback controllo se l'utente presente nel feedback è oggetto di transazione
            Transaction tran = await db.Transactions.Where(p => p.TransactionId == feedback.TransactionId).FirstOrDefaultAsync();
            if( tran.UserProponent_Id == feedback.UserId || tran.UserReceiving_Id == feedback.UserId)
            {
                db.Feedbacks.Add(feedback);
                await db.SaveChangesAsync();
            }
            else
            {
                BadRequest("Non è possibile assegnare un feedback ad un utente che non ha partecipato alla transazione");
            }

            return CreatedAtRoute("DefaultApi", new { id = feedback.FeedbackId }, feedback);
        }

        // DELETE: api/Feedbacks/5
        [ResponseType(typeof(Feedback))]
        public async Task<IHttpActionResult> DeleteFeedback(int id)
        {
            Feedback feedback = await db.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            db.Feedbacks.Remove(feedback);
            await db.SaveChangesAsync();

            return Ok(feedback);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FeedbackExists(int id)
        {
            return db.Feedbacks.Count(e => e.FeedbackId == id) > 0;
        }
    }
}