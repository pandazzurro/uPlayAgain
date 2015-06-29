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
    public class SearchController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/Search/5
        public async Task<IHttpActionResult> Get(int userId, int gameId, int platformId, int genreId, double distance)
        {
            return Ok();
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
