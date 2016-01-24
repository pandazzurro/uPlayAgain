using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.Data.Utils;
using uPlayAgain.Http;
using uPlayAgain.Http.TheGamesDB;

namespace uPlayAgain.GameImporter.Service
{
    public class ConnectionWebApi : IConnectionWebApi
    {
        private IConfigurationApplication _configurationApplication { get; set; }
        private InternalApi _client;
        private InternalApi Client
        {
            get
            {
                if (_clientTheGamesDB == null)
                {
                    _client = new InternalApi(_configurationApplication.GetConfig().BaseInternalApi);
                    Genres = _client.GetGenres().ToList();
                    Platforms = _client.GetPlatforms().ToList();
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
                    _clientTheGamesDB = new TheGamesDBImport(Genres, Platforms);
                return _clientTheGamesDB;
            }
        }
        public List<Genre> Genres { get; set; }
        public List<Platform> Platforms { get; set; }


        public ConnectionWebApi(IConfigurationApplication config)
        {
            _configurationApplication = config;                      
        }

        #region [ Internal API ]
        public async Task<IEnumerable<Genre>> GetGenres() => await _client.GetGenresAsync();
        public async Task<IEnumerable<Platform>> GetPlatformsAsync() => await _client.GetPlatformsAsync();
        public async Task<IEnumerable<Game>> GetAllGame() => await _client.GetGameAll();
        public async Task<Game> GetGameByFieldSearch(Game g) => await _client.GetGameByFieldSearch(g);
        public async Task<IEnumerable<Game>> GetGamesByFieldSearch(Game g) => await _client.GetGamesByFieldSearch(g);
        public async Task<Game> InsertGame(Game g) => await _client.InsertGame(g);
        public async Task<Game> UpdateGame(Game g) => await _client.UpdateGame(g);
        public async Task DeleteGame(Game g) => await _client.DeleteGame(g);
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
