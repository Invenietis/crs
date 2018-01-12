using CK.Crs.AspNetCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class AspNetFeatureExtensions
    {
        public static HttpContext GetHttpContext( this ICommandContext context )
            => context.GetFeature<IHttpContextCommandFeature>()?.HttpContext;
    }
}
