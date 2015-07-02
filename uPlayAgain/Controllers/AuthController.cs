using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class AuthController : ApiController
    {
        uPlayAgainContext db = new uPlayAgainContext();
        
        public async Task<IHttpActionResult> PostAuth(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User userLogin = await db.Users.AsQueryable()
                                     .Where(t => string.Compare(t.Username, user.Username, true) == 0)
                                     .Where(t => string.Compare(t.Password, user.Password, true) == 0)
                                     .FirstOrDefaultAsync();
            if(userLogin != null)
            {
                // impongo un orario di aggiornamento se dal Json non arriva.
                if(userLogin.LastLogin == DateTimeOffset.MinValue)
                    userLogin.LastLogin = DateTimeOffset.Now;

                db.Entry(userLogin).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Ok(userLogin);
            }
            return NotFound();
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
