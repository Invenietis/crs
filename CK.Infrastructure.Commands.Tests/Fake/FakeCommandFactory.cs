using System;
using System.IO;

namespace CK.Infrastructure.Commands.Tests
{
    internal class FakeCommandFactory : ICommandFactory
    {
        public ICommand CreateCommand( CommandRouteRegistration routeInfo, Stream requestPayload )
        {
            if( routeInfo.CommandType == null ) return null;

            return (ICommand)Activator.CreateInstance( routeInfo.CommandType );
        }
    }
}