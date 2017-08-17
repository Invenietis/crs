using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class CompositeCommandReceiver : ICommandReceiver
    {
        IEnumerable<ICommandReceiver> _receivers;
        public CompositeCommandReceiver( IEnumerable<ICommandReceiver> receivers )
        {
            _receivers = receivers;
        }

        public async Task<Response> ReceiveCommand<T>( T command, ICommandContext context ) where T : class
        {
            foreach( var receiver in _receivers )
            {
                var response = await receiver.ReceiveCommand( command, context ).ConfigureAwait( false );
                if( response == null ) continue;

                return response;
            }

            return new ErrorResponse( $"There is no CommandReceivers capable to receive this command: {context.Model.Name}", context.CommandId );
        }
    } 
}
