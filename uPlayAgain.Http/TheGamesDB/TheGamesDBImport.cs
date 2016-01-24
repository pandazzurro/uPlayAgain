using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using Entity = uPlayAgain.TheGamesDB.Entity;

namespace uPlayAgain.Http.TheGamesDB
{
    public class TheGamesDBImport
    {
        public LoadGameList _loadGameList;
        public List<Genre> _genres { get; set; }
        public List<Platform> _platforms { get; set; }
        private Configuration _config { get; set; }        

        /// <summary>
        /// Imposto i generi e le piattaforme gestite da uPlayAgain
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        public TheGamesDBImport(List<Genre> g, List<Platform> p, string UrlGameDetail, string UrlGameList)
        {
            _loadGameList = new LoadGameList();
            _genres = g;
            _platforms = p;
            _config = new Configuration { UrlGameDetail = UrlGameDetail, UrlGameList = UrlGameList };
        }

        /// <summary>
        /// Carica tutta la lista dei giochi per la piattaforma e le date selezionate
        /// </summary>
        /// <param name="dataInizio"></param>
        /// <param name="dataFine"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<List<GameSummary>> LoadGameListByPlatform(DateTime? dataInizio, DateTime? dataFine, Platform p)
        {
            string urlGamePlatform = string.Format("{0}{1}", _config.UrlGameList, p.ImportId);
            return Task.Factory.StartNew(() =>
            {
                return new LoadGameList().Load(urlGamePlatform, p.ImportId, dataInizio, dataFine);
            }).Result;
        }

        /// <summary>
        /// Carica i dati di un singolo gioco presente sul sito 
        /// </summary>
        /// <param name="simpleGame"></param>
        /// <returns></returns>
        public async Task<Game> GetGameDetails(GameSummary simpleGame)
        {
            string urlGameDetails = string.Format("{0}{1}", _config.UrlGameDetail, simpleGame.ID);
            Entity.Data gameDetail = new LoadGameDetails().LoadDetails(urlGameDetails);
            return await Task.Factory.StartNew(() =>
            {
                return GameDetailsToGameDb.Convert(gameDetail, _genres, _platforms);
            });
        }
    }
}
