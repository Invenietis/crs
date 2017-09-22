using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    class NoopClientDispatcher : IClientDispatcher
    {
        public void Send<T>( string callerId, Response<T> response )
        {
            Console.WriteLine( "=== CLIENT DISPATCHER ===" );
            Console.WriteLine( response.RequestId.ToString() );
        }

        public void Broadcast<T>( Response<T> response )
        {
            Console.WriteLine( "=== CLIENT DISPATCHER ===" );
            Console.WriteLine( response.RequestId.ToString() );
        }

        public void Dispose()
        {
        }
    }
}
