using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using uPlayAgain.Data.EF.Models;
using uPlayAgain.Odata;

namespace uPlayAgain
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable cors
            config.EnableCors();

            // Servizi e configurazione dell'API Web
            uPlayAgainOData builder = new uPlayAgainOData();

            // Route dell'API Web
            config.MapHttpAttributeRoutes();

            config.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            JsonMediaTypeFormatter jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
            config.Formatters.Clear();
            config.Formatters.Add(jsonFormatter);
        }
    }
}
