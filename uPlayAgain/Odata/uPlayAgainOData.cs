using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.OData.Builder;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.Odata
{
    public class uPlayAgainOData : ODataConventionModelBuilder
    {
        public uPlayAgainOData()
        {
            EntitySet<Game>("GamesImporter");
            EntitySet<Genre>("GenresImporter");
            EntitySet<Platform>("PlatformsImporter");
        }        
    }
}