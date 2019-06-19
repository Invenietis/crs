using System;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.AspNetCore
{
    internal class HttpContextCommandFeature : IHttpContextCommandFeature, IRequestServicesCommandFeature
    {
        public HttpContextCommandFeature( HttpContext httpContext )
        {
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; }

        public IServiceProvider RequestServices => HttpContext.RequestServices;
    }
}
