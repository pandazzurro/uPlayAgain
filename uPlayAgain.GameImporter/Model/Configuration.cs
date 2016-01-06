using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPlayAgain.GameImporter.Model
{
    public class Configuration
    {
        public string uPlayAgainApi{get; set;}
        public string ExternalApi { get; set; }
        public List<WebApiEndPoint> EndPoints { get; set; }
    }
}
