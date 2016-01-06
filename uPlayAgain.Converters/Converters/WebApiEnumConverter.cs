using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPlayAgain.Data.Utils.Converters
{
    public class WebApiEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject token = JObject.Load(reader);
            WebApiMethod converted = WebApiMethod.GET;
            switch (token.ToString())
            {
                case "GET":
                    {
                        converted = WebApiMethod.GET;
                        break;
                    }
                case "PUT":
                    {
                        converted = WebApiMethod.PUT;
                        break;
                    }
                case "POST":
                    {
                        converted = WebApiMethod.POST;
                        break;
                    }
                case "DELETE":
                    {
                        converted = WebApiMethod.DELETE;
                        break;
                    }
                default:
                    break;
            }
            return converted;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Base serialization is fine
            serializer.Serialize(writer, value);
        }
    }
}
