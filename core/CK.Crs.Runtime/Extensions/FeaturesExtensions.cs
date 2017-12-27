using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class FeaturesExtensions
    {
        public static IServiceProvider GetRequestServices( this ICommandContext context )
        {
            var requestServices = context.GetFeature<IRequestServicesCommandFeature>();
            if( requestServices == null ) return null;

            return requestServices.RequestServices;
        }
    }
}
