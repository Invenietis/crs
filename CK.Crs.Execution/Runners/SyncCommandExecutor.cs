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
    class SyncCommandExecutor : AbstractCommandExecutor
    {
        public SyncCommandExecutor( ICommandHandlerFactory factory, ICommandRegistry registry ) : base( factory, registry )
        {
        }


        protected override bool CanExecute( IPipeline pipeline, CommandDescription commandDescription )
        {
            return true;
        }

        protected override async Task<CommandResponse> ExecuteAsync( IPipeline pipeline, CommandContext context )
        {
            var mon = context.ExecutionContext.Monitor;
            try
            {
                await CommandRunner.ExecuteAsync( context, Factory );
                mon.Trace().Send( "Done." );
            }
            catch( Exception ex )
            {
                mon.Error().Send( ex );
                context.SetException( ex );
            }

            var response = CreateFromContext( context );
            Debug.Assert( response.ResponseType != (char)CommandResponseType.Asynchronous );
            return response;
        }
    }
}
