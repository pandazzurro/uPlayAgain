using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace uPlayAgain.Converters
{
    public class DbGeographyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject location = JObject.Load(reader);
            JToken geography = location.GetValue("GEOGRAPHY", StringComparison.OrdinalIgnoreCase);
            JEnumerable<JToken> geographyList = geography.Children();
            string wellKnownText = String.Empty;
            int coordinateSystemId = 4326;

            foreach (JToken t in geographyList)
            {
                if (t.Path.ToUpper().Contains("wellKnownText".ToUpper()))
                    wellKnownText = t.First.ToString();
                else if (t.Path.ToUpper().Contains("coordinateSystemId".ToUpper()))
                        coordinateSystemId = Convert.ToInt32(t.First.ToString());                
            }

            DbGeography converted = DbGeography.PointFromText(wellKnownText, coordinateSystemId);
            return converted;
        }        

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Base serialization is fine
            serializer.Serialize(writer, value);
        }
    }
}