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
    public class ProposalComponentsController : BaseController
    {
        // GET: api/ProposalComponents
        public IQueryable<ProposalComponent> GetProposalComponents()
        {
            return db.ProposalComponents;
        }

        // GET: api/ProposalComponents/5
        [ResponseType(typeof(ProposalComponent))]
        public async Task<IHttpActionResult> GetProposalComponent(int id)
        {
            ProposalComponent proposalComponent = await db.ProposalComponents.FindAsync(id);
            if (proposalComponent == null)
            {
                return NotFound();
            }

            return Ok(proposalComponent);
        }

        // PUT: api/ProposalComponents/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProposalComponent(int id, ProposalComponent proposalComponent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != proposalComponent.ProposalComponentId)
            {
                return BadRequest();
            }

            db.Entry(proposalComponent).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProposalComponentExists(id))
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

        // POST: api/ProposalComponents
        [ResponseType(typeof(ProposalComponent))]
        public async Task<IHttpActionResult> PostProposalComponent(ProposalComponent proposalComponent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProposalComponents.Add(proposalComponent);
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
                _log.Error(ex);
            }

            return CreatedAtRoute("DefaultApi", new { id = proposalComponent.ProposalComponentId }, proposalComponent);
        }

        // DELETE: api/ProposalComponents/5
        [ResponseType(typeof(ProposalComponent))]
        public async Task<IHttpActionResult> DeleteProposalComponent(int id)
        {
            ProposalComponent proposalComponent = await db.ProposalComponents.FindAsync(id);
            if (proposalComponent == null)
            {
                return NotFound();
            }

            db.ProposalComponents.Remove(proposalComponent);
            await db.SaveChangesAsync();

            return Ok(proposalComponent);
        }
        
        private bool ProposalComponentExists(int id)
        {
            return db.ProposalComponents.Count(e => e.ProposalComponentId == id) > 0;
        }
    }
}