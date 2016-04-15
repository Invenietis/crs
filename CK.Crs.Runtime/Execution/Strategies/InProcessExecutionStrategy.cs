using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    class InProcessExecutionStrategy : IExecutionStrategy
    {
        public async Task<CommandResponse> ExecuteAsync( CommandContext context )
        {
            var mon = context.ExecutionContext.Monitor;
            try
            {
                await CommandRunner.ExecuteAsync( context );
                mon.Trace().Send( "Done." );
            }
            catch( Exception ex )
            {
                mon.Error().Send( ex );
                context.SetException( ex );
            }

            var response = CreateFromContext( context );
            Debug.Assert( response.ResponseType != CommandResponseType.Deferred );
            return response;
        }

        internal static CommandResponse CreateFromContext( CommandContext context )
        {
            if( context.IsDirty )
            {
                if( context.Exception != null )
                    return new CommandErrorResponse( context.Exception.Message, context.ExecutionContext.Action.CommandId );

                if( context.Result != null )
                {
                    return new CommandResultResponse( context.Result, context.ExecutionContext );
                }
            }

            return new CommandDeferredResponse( context.ExecutionContext );
        }

    }
}
