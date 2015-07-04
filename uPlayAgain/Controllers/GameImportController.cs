using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using uPlayAgain.Models;

namespace uPlayAgain.Controllers
{
    public class GameImportController : ApiController
    {
        private uPlayAgainContext db = new uPlayAgainContext();

        // GET: api/GameImporter/5
        public async Task<IHttpActionResult> Get(int id)
        {
            Game game = await db.Games.Where(t => t.ImportId == id).FirstAsync();

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }
        
    }
}
