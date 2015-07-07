using System;
using System.Data.Entity.Spatial;
using System.Web.Http.Validation;

namespace uPlayAgain
{
    
    public class CustomBodyModelValidator : DefaultBodyModelValidator
    {
        public override bool ShouldValidateType(Type type)
        {
            return type != typeof(DbGeography) && base.ShouldValidateType(type);
        }
    }
    
}