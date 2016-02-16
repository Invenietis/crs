using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace CK.Infrastructure.Commands.Tests.Integration
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

        public Task OnCommandReceived( CommandExecutionContext executionContext )
        {
            return Task.FromResult<object>( null );
        }
    }

    internal class AuthorizedFilter : ICommandFilter
    {
        public int Order
        {
            get
            {
                return 0;
            }
        }

        public Task OnCommandReceived( CommandExecutionContext executionContext )
        {
            return Task.FromResult<object>( null );
        }
    }
}