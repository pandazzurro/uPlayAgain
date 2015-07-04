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
    public class TransactionStatusController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/TransactionStatus
        public IQueryable<TransactionStatus> GetTransactionStatus()
        {
            return db.TransactionStatus;
        }

        // GET: api/TransactionStatus/5
        [ResponseType(typeof(TransactionStatus))]
        public async Task<IHttpActionResult> GetTransactionStatus(int id)
        {
            TransactionStatus transactionStatus = await db.TransactionStatus.FindAsync(id);
            if (transactionStatus == null)
            {
                return NotFound();
            }

            return Ok(transactionStatus);
        }

        // PUT: api/TransactionStatus/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTransactionStatus(int id, TransactionStatus transactionStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transactionStatus.TransactionStatusId)
            {
                return BadRequest();
            }

            db.Entry(transactionStatus).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionStatusExists(id))
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

        // POST: api/TransactionStatus
        [ResponseType(typeof(TransactionStatus))]
        public async Task<IHttpActionResult> PostTransactionStatus(TransactionStatus transactionStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TransactionStatus.Add(transactionStatus);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = transactionStatus.TransactionStatusId }, transactionStatus);
        }

        // DELETE: api/TransactionStatus/5
        [ResponseType(typeof(TransactionStatus))]
        public async Task<IHttpActionResult> DeleteTransactionStatus(int id)
        {
            TransactionStatus transactionStatus = await db.TransactionStatus.FindAsync(id);
            if (transactionStatus == null)
            {
                return NotFound();
            }

            db.TransactionStatus.Remove(transactionStatus);
            await db.SaveChangesAsync();

            return Ok(transactionStatus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionStatusExists(int id)
        {
            return db.TransactionStatus.Count(e => e.TransactionStatusId == id) > 0;
        }
    }
}