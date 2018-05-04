using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    /// <summary>
    /// Runtime internal feature extensions
    /// </summary>
    internal static class FeaturesExtensions
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> bounds to the current request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IServiceProvider GetRequestServices( this ICommandContext context )
        {
            var requestServices = context.GetFeature<IRequestServicesCommandFeature>();
            if( requestServices == null ) return null;

            return requestServices.RequestServices;
        }
    }
}
