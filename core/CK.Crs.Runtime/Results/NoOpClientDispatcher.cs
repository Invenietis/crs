using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    class NoopClientDispatcher : IClientDispatcher
    {
        public void Send( string callerId, Response response )
        {
            Console.WriteLine( "=== CLIENT DISPATCHER ===" );
            Console.WriteLine( response.RequestId.ToString() );
        }

        public void Broadcast( Response response )
        {
            Console.WriteLine( "=== CLIENT DISPATCHER ===" );
            Console.WriteLine( response.RequestId.ToString() );
        }

        public void Dispose()
        {
        }
    }
}
