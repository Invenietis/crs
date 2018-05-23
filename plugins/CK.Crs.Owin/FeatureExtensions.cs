using CK.Crs.Owin;
using Microsoft.Owin;

namespace CK.Crs
{
    public static class AspNetFeatureExtensions
    {
        public static IOwinContext GetHttpContext( this ICommandContext context )
            => context.GetFeature<IOwinContextCommandFeature>()?.OwinContext;
    }
}
