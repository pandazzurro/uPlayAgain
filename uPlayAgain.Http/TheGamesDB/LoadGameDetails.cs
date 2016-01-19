using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Entity = uPlayAgain.TheGamesDB.Entity;

namespace uPlayAgain.Http.TheGamesDB
{
    public class LoadGameDetails
    {
        private WebClient _client;
        private const string frontAttribute = "front";
        private Logger _log;        

        public LoadGameDetails()
        {
            _client = new WebClient();
            _log = LogManager.GetLogger("applicationLog");
        }

        public Entity.Data LoadDetails(string url)
        {
            StringReader reader = null;
            Entity.Data DetailGame = null;
            try
            {   
                string xmlString = _client.DownloadString(url);
                XmlSerializer serializer = new XmlSerializer(typeof(Entity.Data));
                reader = new StringReader(xmlString);                
                DetailGame = (Entity.Data)serializer.Deserialize(reader);
                return DetailGame;
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Errore durante lo scaricamento delle informazioni del gioco presente all'url {0}", url);                
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            return DetailGame;
        }

        public Entity.Data DownloadImage(Entity.Data detailGame)
        {
            try
            {
                string frontImage = detailGame.Game.Images.boxart
                                                    .Where(x => x.side.Equals(frontAttribute))
                                                    .Select(y => y.Value)
                                                    .FirstOrDefault();
                if (!string.IsNullOrEmpty(frontImage))
                {
                    string url = string.Format("{0}{1}", detailGame.baseImgUrl, frontImage);
                    detailGame.DowloadedFrontImage = _client.DownloadData(new Uri(url));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Errore durante lo scaricamento dell'immagine del gioco presente in: {0}", detailGame.baseImgUrl);
            }
            return detailGame;
        }
    }
}
