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
    public class ProposalsController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

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
                                        .Include(t => t.ProposalComponents)                                        
                                        .Include(t => t.UserLastChanges)
                                        .Where(t => t.ProposalId == id)
                                        .SingleOrDefaultAsync();
            if (proposal == null)
            {
                return NotFound();
            }
            await db.Libraries.LoadAsync();
            await db.LibraryComponents.LoadAsync();
            await db.GameLanguages.LoadAsync();
            await db.Games.LoadAsync();
            await db.Genres.LoadAsync();
            await db.Platforms.LoadAsync();

            return Ok(proposal);
        }

        // PUT: api/Proposals/5
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
            await db.SaveChangesAsync();

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProposalExists(int id)
        {
            return db.Proposals.Count(e => e.ProposalId == id) > 0;
        }
    }
}