using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.AspNetCore
{
    public interface IHttpContextCommandFeature
    {
        /// <summary>
        /// Sets the <paramref name="httpContext"/>. This makes the object lifetime dependent of the request.
        /// </summary>
        /// <param name="httpContext"></param>
        HttpContext HttpContext { get; }
    }
}
