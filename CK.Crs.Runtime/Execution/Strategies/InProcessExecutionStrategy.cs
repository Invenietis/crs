using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
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
                context.SetException( ex );
            }

            var response = CommandResponse.CreateFromContext( context );
            Debug.Assert( response.ResponseType != CommandResponseType.Deferred );
            return response;
        }
    }
}
