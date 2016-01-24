using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace uPlayAgain.Http.TheGamesDB
{
    public class LoadGameList
    {
        public const string Game = "Game";
        public const string Id = "id";
        public const string Title = "GameTitle";
        public const string Release = "ReleaseDate";
        public WebClient _client;
        private Logger _log;

        public LoadGameList()
        {
            _client = new WebClient();
            _log = LogManager.GetLogger("applicationLog");
        }

        public List<GameSummary> Load(string url, int idPlatform, DateTime? dataInizio, DateTime? dataFine)
        {
            List<GameSummary> GamesInPlatform = new List<GameSummary>();
            try
            {   
                string xmlString = _client.DownloadString(url);
                XmlTextReader reader = new XmlTextReader(new StringReader(xmlString));
                GameSummary game = null;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {

                            case Game:
                                {
                                    game = new GameSummary();
                                    game.Platform = idPlatform;
                                    break;
                                }
                            case Id:
                                {
                                    game.ID = reader.ReadElementContentAsInt();
                                    break;
                                }
                            case Title:
                                {
                                    game.Title = reader.ReadElementContentAsString();
                                    break;
                                }
                            case Release:
                                {
                                    string date = reader.ReadElementContentAsString();
                                    game.Release = Convert.ToDateTime(date, new System.Globalization.CultureInfo("en-US", true));
                                    break;
                                }
                            default:
                                { break; }
                        }
                    }
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == Game)
                    {
                        if ((dataInizio.HasValue && dataFine.HasValue && game.Release <= dataFine.Value && game.Release >= dataInizio.Value)
                            || (!dataInizio.HasValue && !dataFine.HasValue))
                        {
                            if (game.Platform > 0 && !string.IsNullOrEmpty(game.Title))
                                GamesInPlatform.Add(game);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Errore durante lo scaricamento delle informazioni di sommario dei giochi presenti: {0}", url);                
            }
            return GamesInPlatform;
        }
    }
}
