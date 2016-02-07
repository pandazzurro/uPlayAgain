using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.Data.Utils;
using uPlayAgain.Http;
using uPlayAgain.Http.TheGamesDB;

namespace uPlayAgain.GameImporter.Service
{
    public class ConnectionWebApi : IConnectionWebApi
    {
        private IConfigurationApplication _configurationApplication;
        private InternalApi _client;
        private InternalApi Client
        {
            get
            {
                if (_clientTheGamesDB == null)
                {
                    _client = new InternalApi(_configurationApplication.GetConfig().BaseInternalApi);
                }
                return _client; 
            }
        }
        private TheGamesDBImport _clientTheGamesDB;
        protected TheGamesDBImport ClientTheGamesDB
        {
            get
            {
                if(_clientTheGamesDB == null)
                {
                    _clientTheGamesDB = new TheGamesDBImport(
                        Client.GetGenres().ToList(),
                        Client.GetPlatforms().ToList(), 
                        _configurationApplication.GetConfig().TheGamesDBGameDetail, 
                        _configurationApplication.GetConfig().TheGamesDBGameList);
                }   
                return _clientTheGamesDB;
            }
        }

        public ConnectionWebApi(IConfigurationApplication config)
        {
            _configurationApplication = config;                      
        }

        #region [ Internal API ]
        public IEnumerable<Genre> GetGenres() => Client.GetGenres();
        public IEnumerable<Platform> GetPlatforms() => Client.GetPlatforms();
        public async Task<IEnumerable<Platform>> GetPlatformsAsync() => await Client.GetPlatformsAsync();
        public async Task<IEnumerable<Genre>> GetGenresAsync() => await Client.GetGenresAsync();
        public async Task<IEnumerable<Game>> GetAllGame() => await Client.GetGameAll();
        public async Task<Game> GetGameByFieldSearch(Game g) => await Client.GetGameByFieldSearch(g);
        public async Task<IEnumerable<Game>> GetGamesByFieldSearch(Game g) => await Client.GetGamesByFieldSearch(g);
        public async Task<IEnumerable<Game>> GetGameIds(Game g) => await Client.GetGameIds(g);
        public async Task<Game> GetGameById(Game g) => await Client.GetGameById(g);
        public async Task<Game> InsertGame(Game g) => await Client.InsertGame(g);
        public async Task<Game> UpdateGame(Game g) => await Client.UpdateGame(g);
        public async Task DeleteGame(Game g) => await Client.DeleteGame(g);
        #endregion

        #region [ TheGamesDB ]
        public async Task<List<GameSummary>> TheGamesDBGameListByPlatform(DateTime? dataInizio, DateTime? dataFine, Platform platform)
        {
            return await ClientTheGamesDB.LoadGameListByPlatform(dataInizio, dataFine, platform);            
        }

        public async Task<Game> TheGamesDBGetGameDetails(GameSummary gameSummary)
        {
            return await ClientTheGamesDB.GetGameDetails(gameSummary);
        }
        #endregion
    }
}
