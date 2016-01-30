using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.TheGamesDB.Entity;

namespace uPlayAgain.Http.TheGamesDB
{
    public static class GameDetailsToGameDb
    {
        /// <summary>
        /// Converte i dati di un gioco presente su TheGamesDB con quelli di uPlayAgain
        /// </summary>
        /// <param name="d"></param>
        /// <param name="genres"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public static Game Convert(uPlayAgain.TheGamesDB.Entity.Data d, List<Genre> genres, List<Platform> platforms)
        {
            Game g = new Game();            
            g.ImportId = d.Game.id;
            g.Description = d.Game.Overview;
            g.Title = d.Game.GameTitle;
            if (!string.IsNullOrEmpty(d.Game.GameTitle))
                g.ShortName = d.Game.GameTitle.RemoveMultipleSpace().Replace(" ", "-").Substring(0, Math.Min(d.Game.GameTitle.Length, 30));
            g.Platform = GetPlatform(d.Game.PlatformId, platforms);
            g.PlatformId = g.Platform != null ? g.Platform.PlatformId : null;
            g.Image = d.DowloadedFrontImage;
            g.ImageThumb = g.Resize();
            g.Genre = GetGenre(d.Game.Genres == null ? string.Empty : d.Game.Genres.genre, genres);
            g.GenreId = g.Genre != null ? g.Genre.GenreId : null;
            return g;
        }

        /// <summary>
        /// Mappatura Generi di importazione
        /// </summary>
        /// <param name="genre"></param>
        /// <param name="genres"></param>
        /// <returns></returns>
        private static Genre GetGenre(string genre, List<Genre> genres)
        {
            Genre genreResult = null;
            string newGenre = string.Empty;
            switch (genre.ToUpper())
            {
                case "ACTION":
                    {
                        newGenre = "action";
                        break;
                    }
                case "ROLE-PLAYING":
                    {
                        newGenre = "rpg";
                        break;
                    }
                case "SHOOTER":
                    {
                        newGenre = "fps";
                        break;
                    }
                case "ADVENTURE":
                    {
                        newGenre = "adventure";
                        break;
                    }
                case "RACING":
                    {
                        newGenre = "racing";
                        break;
                    }
                case "PLATFORM":
                    {
                        newGenre = "platform";
                        break;
                    }
                case "FIGHTING":
                    {
                        newGenre = "beatemup";
                        break;
                    }
                case "SPORTS":
                    {
                        newGenre = "sport";
                        break;
                    }
                case "LIFE SIMULATION":
                case "SANDBOX":
                    {
                        newGenre = "simulation";
                        break;
                    }
                case "STEALTH":
                    {
                        newGenre = "action";
                        break;
                    }
                case "MMO":
                    {
                        newGenre = "mmo";
                        break;
                    }
                case "STRATEGY":
                    {
                        newGenre = "strategy";
                        break;
                    }
                case "HORROR":
                    {
                        newGenre = "horror";
                        break;
                    }
                case "PUZZLE":
                case "MUSIC":
                    {
                        newGenre = "other";
                        break;
                    }
                default:
                    {
                        genreResult = genres.Find(x => x.GenreId.ToUpper() == "FPS");
                        break;
                    }
            }
            genreResult = genres.Find(x => x.GenreId.ToUpper() == newGenre.ToUpper());
            
            if (genreResult == null)
            {
                Logger _log = LogManager.GetLogger("applicationLog");
                _log.Error(string.Format("Nessun genere associato per il genere {0}", genre));
            }

            return genreResult;
        }

        /// <summary>
        /// Mappatura piattaforme
        /// </summary>
        /// <param name="idPlatform"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        private static Platform GetPlatform(int idPlatform, List<Platform> platforms)
        {
            return platforms.Where(pl => pl.ImportId == idPlatform)
                            .FirstOrDefault();
        }

        private static string RemoveMultipleSpace(this string word)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            word = regex.Replace(word, @" ");
            return word;
        }
    }
}
