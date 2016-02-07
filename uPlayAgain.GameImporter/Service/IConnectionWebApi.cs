using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.Http.TheGamesDB;

namespace uPlayAgain.GameImporter.Service
{
    public interface IConnectionWebApi
    {
        #region [ Internal API ]
        IEnumerable<Genre> GetGenres();
        IEnumerable<Platform> GetPlatforms();
        Task<IEnumerable<Platform>> GetPlatformsAsync();
        Task<IEnumerable<Genre>> GetGenresAsync();
        Task<IEnumerable<Game>> GetAllGame();
        Task<Game> GetGameByFieldSearch(Game g);
        Task<IEnumerable<Game>> GetGamesByFieldSearch(Game g);
        Task<IEnumerable<Game>> GetGameIds(Game g);
        Task<Game> GetGameById(Game g);
        Task<Game> InsertGame(Game g);
        Task<Game> UpdateGame(Game g);
        Task DeleteGame(Game g);
        #endregion

        #region [ TheGamesDB ]
        Task<List<GameSummary>> TheGamesDBGameListByPlatform(DateTime? dataInizio, DateTime? dataFine, Platform platform);
        Task<Game> TheGamesDBGetGameDetails(GameSummary gameSummary);
        #endregion
    }
}
