using Microsoft.AspNet.SignalR;
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
using uPlayAgain.Hubs;

namespace uPlayAgain.Controllers
{
    public class ProposalsController : BaseController
    {
        // GET: api/Proposals
        public IQueryable<Proposal> GetProposals()
        {
            return db.Proposals
                     .Include(t => t.ProposalComponents)
                     .Include(t => t.UserLastChanges);
        }

        // GET: api/Proposals/5
        [ResponseType(typeof(Proposal))]
        public async Task<IHttpActionResult> GetProposal(int id)
        {
            Proposal proposal = await db.Proposals
                                        .Include(x => x.Transaction)
                                        .Where(t => t.ProposalId == id)
                                        .SingleOrDefaultAsync();
            if (proposal == null)
            {
                return NotFound();
            }

            return Ok(proposal);
        }

        // PUT: api/Proposals/5
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProposal(int id, Proposal proposal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != proposal.ProposalId)
            {
                return BadRequest();
            }

            db.Entry(proposal).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProposalExists(id))
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

        // POST: api/Proposals
        [ResponseType(typeof(Proposal))]
        public async Task<IHttpActionResult> PostProposal(Proposal proposal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Proposals.Add(proposal);
            db.LibraryComponents.Load();
            db.Transactions.Load();
            db.Users.Load();
            try
            {
                await db.SaveChangesAsync();

                // Notifico SOLO al client specifico che è stato inviato un messaggio 
                string userToSend = string.Empty;
                if (proposal.Direction)
                    userToSend = proposal.Transaction.UserReceiving_Id;
                else
                    userToSend = proposal.Transaction.UserProponent_Id;
                

                IHubContext ctx = GlobalHost.ConnectionManager.GetHubContext<MessageConnection>();                
                string ConnectionId = MessageConnection.GetConnectionByUserID(userToSend);
                await ctx.Clients.Client(ConnectionId).sendProposalHub(proposal, ConnectionId);
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
                _log.Error(ex);
            }
            

            return CreatedAtRoute("DefaultApi", new { id = proposal.ProposalId }, proposal);
        }

        // DELETE: api/Proposals/5
        [ResponseType(typeof(Proposal))]
        public async Task<IHttpActionResult> DeleteProposal(int id)
        {
            Proposal proposal = await db.Proposals.FindAsync(id);
            if (proposal == null)
            {
                return NotFound();
            }

            db.Proposals.Remove(proposal);
            await db.SaveChangesAsync();

            return Ok(proposal);
        }
        
        private bool ProposalExists(int id)
        {
            return db.Proposals.Count(e => e.ProposalId == id) > 0;
        }
    }
}