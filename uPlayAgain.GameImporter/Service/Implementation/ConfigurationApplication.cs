using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using uPlayAgain.GameImporter.Model;
using uPlayAgain.Data.Utils;

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
