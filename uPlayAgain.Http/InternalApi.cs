using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using Simple.OData.Client;
using Newtonsoft.Json;
using System.IO;

namespace uPlayAgain.Http
{
    public class InternalApi
    {
        private ODataClient _client;        

        public InternalApi(string uri)
        {   
            _client = new ODataClient(uri);           
        }

        public IEnumerable<Genre> GetGenres() => this.GetGenresAsync().Result;

        public IEnumerable<Platform> GetPlatforms() => GetPlatformsAsync().Result;

        public async Task<IEnumerable<Genre>> GetGenresAsync() => await _client.For<Genre>("GenresImporter").FindEntriesAsync();

        public async Task<IEnumerable<Platform>> GetPlatformsAsync() => await _client.For<Platform>("PlatformsImporter").FindEntriesAsync();

        public async Task<IEnumerable<Game>> GetGameAll() => await _client.For<Game>("GamesImporter").FindEntriesAsync();
        
        public async Task<Game> GetGameByFieldSearch(Game g)
        {
            return await getBoundGameByFieldSearch(g).FindEntryAsync();
        }

        public async Task<IEnumerable<Game>> GetGamesByFieldSearch(Game g)
        {
            return await getBoundGameByFieldSearch(g).FindEntriesAsync();
        }

        private IBoundClient<Game> getBoundGameByFieldSearch(Game g)
        {
            IBoundClient<Game> executor = _client.For<Game>("GamesImporter")
                                                 .Expand(y => new { y.Genre, y.Platform });
            if (!string.IsNullOrEmpty(g.Description))
                executor.Filter(x => x.Description.Contains(g.Description));

            if (!string.IsNullOrEmpty(g.GenreId))
                executor.Filter(x => x.GenreId.Contains(g.GenreId));

            if (!string.IsNullOrEmpty(g.PlatformId))
                executor.Filter(x => x.PlatformId.Contains(g.PlatformId));

            if (!string.IsNullOrEmpty(g.ShortName))
                executor.Filter(x => x.ShortName.Contains(g.ShortName));

            if (!string.IsNullOrEmpty(g.Title))
                executor.Filter(x => x.Title.Contains(g.Title));

            if (g.ImportId.HasValue)
                executor.Filter(x => x.ImportId.Value == g.ImportId.Value);
            return executor;
        }

        public async Task<Game> InsertGame(Game g)
        {
            return await _client.For<Game>("GamesImporter")                                
                                       .Set(g)                                
                                       .InsertEntryAsync(true);
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
