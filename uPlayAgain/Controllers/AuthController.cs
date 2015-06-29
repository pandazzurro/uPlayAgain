using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class AuthController : ApiController
    {
        uPlayAgainContext db = new uPlayAgainContext();
        // GET: api/Auth/5
        public async Task<IHttpActionResult> Get(string username, string password)
        {
            if (db.Users.Where(t => string.Compare(t.Username, username, true) == 0 && string.Compare(t.Password, password, true) == 0).Any())
                return Ok();
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
