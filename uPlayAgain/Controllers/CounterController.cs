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
    public class CounterController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Counter/GamesByUser/5
        [Route("api/Counter/GamesByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetGamesByUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            int result = await db.Libraries
                                 .Include(t => t.LibraryComponents)
                                 .Include(t => t.User)
                                 .Where(t => t.User.UserId == id)
                                 .CountAsync();
            return Ok(result);
        }

        // GET: api/Counter/GamesByUser/5
        [Route("api/Counter/LibrariesByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetLibrariesByUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            int result = await db.Libraries
                                 .Include(t => t.User)
                                 .Where(t => t.User.UserId == id)
                                 .CountAsync();
            return Ok(result);
        }

        // GET: api/Counter/GamesByUser/5
        [Route("api/Counter/MessagesByUser/{id:int}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetMessagesByUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            int result = await db.Messages
                                 .Include(t => t.UserReceiving)
                                 .Where(t => t.UserReceiving.UserId == id && t.IsAlreadyRead == false)
                                 .CountAsync();
            return Ok(result);
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