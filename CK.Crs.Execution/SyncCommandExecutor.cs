using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Execution
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncCommandExecutor : AbstractCommandExecutor
    {
        public SyncCommandExecutor( ICommandExecutionFactories factories ) : base( factories )
        {
        }

        protected override bool CanExecute( IPipeline pipeline, CommandDescription commandDescription )
        {
            return true;
        }
   
        protected override async Task<CommandResponse> ExecuteAsync( CommandContext context )
        {
            var mon = context.ExecutionContext.Monitor;
            try
            {
                await CommandRunner.ExecuteAsync( context, Factories );
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
    }
}
