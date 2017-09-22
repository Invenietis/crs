using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore.Tests
{
    [Route( "crs" )]
    class TestEndpoint<T> : IHttpCommandReceiver<T> where T : class
    {
        public TestEndpoint( ICommandReceiver receiver )
        {
            Receiver = receiver;
        }

        public ICommandReceiver Receiver { get; }

        [HttpPost( "[Action]" )]
        public Task<Response> ReceiveCommand( T command, IHttpCommandContext context )
        {
            return Receiver.ReceiveCommand( command, context );
        }
    }
}
