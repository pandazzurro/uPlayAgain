using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Controllers
{
    public class TransactionsController : BaseController
    {
        // GET: api/Transactions
        public IQueryable<Transaction> GetTransactions()
        {
            return db.Transactions;
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> GetTransaction(int id)
        {
            Transaction transaction = await db.Transactions
                                              .Include(p => p.Proposals)
                                              // Non carico i componenti della proposta. I componenti li posso ricavare da una ulteriore chiamata, filtrando per la proposta desiderata.
                                              .Include(p => p.Feedbacks)
                                              .Where(p => p.TransactionId == id)
                                              .SingleOrDefaultAsync();
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }
        
        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTransaction(int id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            db.Entry(transaction).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        // POST: api/Transactions
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> PostTransaction(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            db.Transactions.Add(transaction);
            db.LibraryComponents.Load();
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                ex.EntityValidationErrors.ToList().ForEach(entityValidation => { entityValidation.ValidationErrors.ToList().ForEach(validation => sb.Append(string.Concat(validation.PropertyName, " - ", validation.ErrorMessage))); });

                _log.Error("{0}{1}Validation errors:{1}{2}", ex, Environment.NewLine, sb.ToString());
                throw;
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            
            

            return CreatedAtRoute("DefaultApi", new { id = transaction.TransactionId }, transaction);
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> DeleteTransaction(int id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Ok(transaction);
        }
        
        private bool TransactionExists(int id)
        {
            return db.Transactions.Count(e => e.TransactionId == id) > 0;
        }
    }
}