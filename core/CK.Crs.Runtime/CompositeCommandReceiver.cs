using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using System.Linq;

namespace CK.Crs
{
    class CompositeCommandReceiver : ICommandReceiver
    {
        readonly IReadOnlyCollection<ICommandReceiver> _receivers;

        public CompositeCommandReceiver( IEnumerable<ICommandReceiver> receivers )
        {
            _receivers = receivers.ToArray();
        }

        public string Name => "CompositeCommandReceiver";

        public bool AcceptCommand( ICommandContext context )
        {
            // By design this command receiver is the root of the other ones.
            return true;
        }

        public Task<Response> ReceiveCommand<T>( T command, ICommandContext context )
        {
            context.Monitor.Info( $"Receiving command {context.Model.Name}..." );
            using( context.Monitor.OpenTrace( $"Determining the best receiver for the command among the {_receivers.Count} available receivers..." ) )
            {
                foreach( var receiver in _receivers )
                {
                    context.Monitor.Trace( $"Challenging receiver {receiver.Name}." );
                    if( receiver.AcceptCommand( context ) )
                    {
                        context.Monitor.Trace( $"Receiver {receiver.Name} accepts to receive the command." );
                        return receiver.ReceiveCommand( command, context );
                    }
                }
                context.Monitor.Warn( $"No receiver accepts to receive the command." );
            }
            var response = new ErrorResponse( $"There is no CommandReceivers capable to receive this command: {context.Model.Name}", context.CommandId );
            return Task.FromResult<Response>( response );
        }
    }
}
