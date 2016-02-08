using System;

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

        public void OnCommandReceived( CommandExecutionContext executionContext )
        {
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

        public void OnCommandReceived( CommandExecutionContext executionContext )
        {
        }
    }
}