﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using Simple.OData.Client;
using Newtonsoft.Json;
using System.IO;
using NLog;
using System.Runtime.ExceptionServices;
using System.Net.Http;

namespace uPlayAgain.Http
{
    public class InternalApi
    {
        private ODataClient _client;
        private Logger _log;

        public InternalApi(string uri)
        {
            _log = LogManager.GetLogger("applicationLog");
            _client = new ODataClient(new ODataClientSettings
            {
                BaseUri = new Uri(uri),
                OnTrace = (x, y) => _log.Info(string.Format(x, y)),
                BeforeRequest = (HttpRequestMessage message) =>
                {
                    message.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36");                    
                }
            });
        }


        public IEnumerable<Genre> GetGenres() => Task.Factory.StartNew(() =>
        {
            return GetGenresAsync().Result;
        }).Result;

        public IEnumerable<Platform> GetPlatforms() => Task.Factory.StartNew(() =>
        {
            return GetPlatformsAsync().Result;
        }).Result;

        public async Task<IEnumerable<Genre>> GetGenresAsync() => await _client.For<Genre>("GenresImporter").FindEntriesAsync().ConfigureAwait(false);

        public async Task<IEnumerable<Platform>> GetPlatformsAsync() => await _client.For<Platform>("PlatformsImporter").FindEntriesAsync();

        public async Task<IEnumerable<Game>> GetGameAll() => await _client.For<Game>("GamesImporter").FindEntriesAsync();
        
        public async Task<Game> GetGameByFieldSearch(Game g)
        {
            return await getBoundGameByFieldSearch(g).FindEntryAsync();
        }

        public async Task<IEnumerable<Game>> GetGameIds(Game g)
        {
            return await getBoundGameByFieldSearch(g, false).Select(x => x.GameId).FindEntriesAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesByFieldSearch(Game g)
        {
            return await getBoundGameByFieldSearch(g).FindEntriesAsync();
        }

        public async Task<Game> GetGameById(Game g)
        {
            return await _client.For<Game>("GamesImporter")
                                .Key(g)
                                .Expand(y => new { y.Genre, y.Platform })
                                .FindEntryAsync();
        }
        private IBoundClient<Game> getBoundGameByFieldSearch(Game g, bool expand = true)
        {
            IBoundClient<Game> executor = _client.For<Game>("GamesImporter");
            if(expand)
                executor.Expand(y => new { y.Genre, y.Platform });

            if (!string.IsNullOrEmpty(g.Description))
                executor.Filter(x => x.Description.Contains(g.Description));

            if (!string.IsNullOrEmpty(g.GenreId))
                executor.Filter(x => x.GenreId == g.GenreId);

            if (!string.IsNullOrEmpty(g.PlatformId))
                executor.Filter(x => x.PlatformId == g.PlatformId);

            if (!string.IsNullOrEmpty(g.ShortName))
                executor.Filter(x => x.ShortName.Contains(g.ShortName));

            if (!string.IsNullOrEmpty(g.Title))
                executor.Filter(x => x.Title.Contains(g.Title));

            if (g.ImportId.HasValue)
                executor.Filter("ImportId eq " + g.ImportId.Value);                
            
            return executor;
        }

        public async Task<Game> InsertGame(Game g)
        {
            try
            {
                return await _client.For<Game>("GamesImporter")
                                       .Set(g)
                                       .InsertEntryAsync();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<Game> UpdateGame(Game g)
        {
            return await _client.For<Game>("GamesImporter")
                                .Key(g)
                                .Set(g)
                                .UpdateEntryAsync(true);
        }

        public async Task DeleteGame(Game g)
        {
            await _client.For<Game>("GamesImporter")
                         .Key(g)
                         .DeleteEntryAsync();
        }
    }
}
