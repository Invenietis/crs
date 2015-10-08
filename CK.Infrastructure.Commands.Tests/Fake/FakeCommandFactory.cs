using System;
using System.IO;

namespace CK.Infrastructure.Commands.Tests
{
    internal class FakeCommandFactory : ICommandFactory
    {
        public object CreateCommand( CommandRouteRegistration routeInfo, Stream requestPayload )
        {
            throw new NotImplementedException();
        }
    }
}