using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class CommandRunner : ICommandRunner
    {
        public CommandRunner( ICommandHandlerFactory factory )
        {
            HandlerFactory = factory;
        }

        public ICommandHandlerFactory HandlerFactory { get; private set; }

        protected class JobResult
        {
            public object Result { get; set; }
            public Exception Exception { get; set; }
            public TimeSpan ElapsedTime { get; set; }
        }

        protected async virtual Task<JobResult> DoJob( Type handlerType, object command )
        {
            JobResult result = new JobResult();
            try
            {
                ICommandHandler handler = HandlerFactory.Create( handlerType );
                result.Result = await handler.HandleAsync( command );
            }
            catch( Exception ex )
            {
                result.Exception = ex;
            }
            return result;
        }

        public virtual async Task<ICommandResponse> RunAsync( CommandProcessingContext ctx, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            JobResult result = await DoJob( ctx.HandlerType, ctx.Request.Command );
            if( result.Exception == null )
            {
                return ctx.CreateDirectResponse( result.Result );
            }
            return ctx.CreateErrorResponse( result.Exception.Message );
        }
    }
}
