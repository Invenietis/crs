using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiver : ICommandReceiver
    {
        readonly ICommandRunnerFactory _runners;

        public DefaultCommandReceiver( ICommandRunnerFactory runnerFactory )
        {
            if( runnerFactory == null ) throw new ArgumentNullException( nameof( runnerFactory ) );

            _runners = runnerFactory;
        }

        public Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            if( commandRequest.CommandDescription.HandlerType == null )
            {
                string msg = "Handler not found for command type " + commandRequest.CommandDescription.CommandType;
                return Task.FromResult<ICommandResponse>( new ErrorResponse( msg, Guid.NewGuid() ) );
            }
            CommandProcessingContext processingContext = new CommandProcessingContext( Guid.NewGuid(), commandRequest );

            return _runners.CreateRunner( processingContext.RuntimeContext ).RunAsync( processingContext, cancellationToken );
        }
    }
}
