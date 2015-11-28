using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Controllers
{
    public class GameImportController : BaseController
    {
        // GET: api/GameImporter/5
        public async Task<IHttpActionResult> Get(int id)
        {
            _log.Debug("GameImporter - Get - id: {0}", id);
            IList<Game> games = await db.Games.Where(t => t.ImportId == id).ToListAsync();
            _log.Debug("GameImporter - presenti già {0} IdImport", games.Count);

            if (games.Count > 1)
            {
                _log.Debug("GameImporter - BadRequest");
                return BadRequest();
            }

            if (!games.Any())
            {
                _log.Debug("GameImporter - NotFound");
                return NotFound();
            }
            _log.Debug("GameImporter - Ok");
            return Ok(games);
        }
        
    }
}
