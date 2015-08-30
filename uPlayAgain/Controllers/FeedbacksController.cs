using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
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
        [Route("api/Feedbacks/Pending/{userId}")]
        [ResponseType(typeof(int))]
        public IQueryable<int> GetTransactionFeedbacksPendings(string userId)
        {
            return db.Transactions
                     // Cerco tutte le transazioni dove sono Proponente o Ricevente e NON ho nessun feedback associato
                     .Where(p => (p.UserProponent_Id == userId || p.UserReceiving_Id == userId) 
                         &&    (!p.Feedbacks.Where(x => x.UserId == userId).Any() || !p.Feedbacks.Any())
                         )
                     .Select( p => p.TransactionId);
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