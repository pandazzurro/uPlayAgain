using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPlayAgain.Data.Utils;
using uPlayAgain.Data.Utils.Converters;

namespace uPlayAgain.GameImporter.Model
{
    public class WebApiEndPoint
    {
        public string Controller { get; set; }

        [JsonConverter(typeof(WebApiEnumConverter))]
        public List<WebApiMethod> Method { get; set; }
    }
}
