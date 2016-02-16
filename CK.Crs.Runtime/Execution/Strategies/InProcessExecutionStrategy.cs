using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    class InProcessExecutionStrategy : IExecutionStrategy
    {
        readonly CommandRunner _runner;
        public InProcessExecutionStrategy( CommandRunner runner )
        {
            _runner = runner;
        }

        public async Task<CommandResponse> ExecuteAsync( CommandContext context )
        {
            var mon = context.Command.Monitor;
            try
            {
                await _runner.ExecuteAsync( context );
                mon.Trace().Send( "Done." );
            }
            catch( Exception ex )
            {
                mon.Error().Send( ex );
            }

            return CommandResponse.CreateFromContext( context );
        }
    }
}
