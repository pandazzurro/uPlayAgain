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
    public class ProposalStatusController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/ProposalStatus
        public IQueryable<ProposalStatus> GetProposalStaus()
        {
            return db.ProposalStaus;
        }

        // GET: api/ProposalStatus/5
        [ResponseType(typeof(ProposalStatus))]
        public async Task<IHttpActionResult> GetProposalStatus(int id)
        {
            ProposalStatus proposalStatus = await db.ProposalStaus.FindAsync(id);
            if (proposalStatus == null)
            {
                return NotFound();
            }

            return Ok(proposalStatus);
        }

        // PUT: api/ProposalStatus/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProposalStatus(int id, ProposalStatus proposalStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != proposalStatus.ProposalStatusId)
            {
                return BadRequest();
            }

            db.Entry(proposalStatus).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProposalStatusExists(id))
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

        // POST: api/ProposalStatus
        [ResponseType(typeof(ProposalStatus))]
        public async Task<IHttpActionResult> PostProposalStatus(ProposalStatus proposalStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProposalStaus.Add(proposalStatus);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = proposalStatus.ProposalStatusId }, proposalStatus);
        }

        // DELETE: api/ProposalStatus/5
        [ResponseType(typeof(ProposalStatus))]
        public async Task<IHttpActionResult> DeleteProposalStatus(int id)
        {
            ProposalStatus proposalStatus = await db.ProposalStaus.FindAsync(id);
            if (proposalStatus == null)
            {
                return NotFound();
            }

            db.ProposalStaus.Remove(proposalStatus);
            await db.SaveChangesAsync();

            return Ok(proposalStatus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProposalStatusExists(int id)
        {
            return db.ProposalStaus.Count(e => e.ProposalStatusId == id) > 0;
        }
    }
}