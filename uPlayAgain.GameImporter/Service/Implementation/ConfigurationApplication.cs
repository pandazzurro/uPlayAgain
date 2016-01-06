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

        public Configuration GetConfig()
        {
            return _configurations;
        }

        public WebApiEndPoint GetEndPoint(string controller, WebApiMethod method)
        {
            return _configurations.EndPoints.Where(x => string.Equals(x.Controller, controller, StringComparison.CurrentCultureIgnoreCase) 
                                            && x.Method.Any(y => string.Equals(Enum.GetName(typeof(WebApiMethod),y), method.ToString(), StringComparison.CurrentCultureIgnoreCase)))
                          .FirstOrDefault();


        }
    }
}
