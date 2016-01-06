using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.Data.Utils;

namespace uPlayAgain.GameImporter.Service
{
    public class ConnectionWebApi : IConnectionWebApi
    {
        private IConfigurationApplication _configurationApplication { get; set; }
        private HttpClient _client { get; set; }

        public ConnectionWebApi(IConfigurationApplication config)
        {
            _configurationApplication = config;
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_configurationApplication.GetConfig().uPlayAgainApi);            
        }

        public async Task<List<Game>> GetAllGame()
        {
            if (_configurationApplication.GetEndPoint(typeof(Game).ToString(), WebApiMethod.GET) != null)
            {
                HttpResponseMessage response = await _client.GetAsync(_configurationApplication.GetEndPoint(typeof(Game).ToString(), WebApiMethod.GET).Controller);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                
            }
            return new List<Game>();  
        }
    }
}
