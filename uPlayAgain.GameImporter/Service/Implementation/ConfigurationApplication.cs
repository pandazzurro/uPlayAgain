using System.IO;
using Newtonsoft.Json;
using uPlayAgain.GameImporter.Model;

namespace uPlayAgain.GameImporter.Service
{
    public class ConfigurationApplication : IConfigurationApplication
    {
        private Configuration _configurations;
        
        public ConfigurationApplication()
        {
            _configurations = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("configuration.json"));
        }

        public Configuration GetConfig() => _configurations;
    }
}
