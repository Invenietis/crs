using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace CK.Crs.Tests.Integration
{
    internal class HttpsRequiredFilter : ICommandFilter
    {
        public int Order
        {
            get
            {
                return 0;
            }
        }

        public Task OnCommandReceived( ICommandFilterContext filterContext )
        {
            return Task.FromResult<object>( null );
        }
    }
}