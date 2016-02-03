using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public class InProcessCommandExecutor : CommandExecutor
    {
        public InProcessCommandExecutor( ICommandReceiverFactories factories ) : base( factories )
        {
        }

        public override async Task ExecuteAsync( CommandExecutionContext exContext, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var mon = exContext.RuntimeContext.Monitor;
            try
            {
                await base.ExecuteAsync( exContext, cancellationToken );
                mon.Trace().Send( "Done." );
            }
            catch( Exception ex )
            {
                mon.Error().Send( ex );
                throw;
            }
        }
    }
}
