using CK.Crs.Samples.Messages;
using Paramore.Brighter;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace CK.Crs.Samples.Handlers
{
    public class SuperHandler : RequestHandlerAsync<SuperCommand>, ICommandHandler<SuperCommand>
    {
        public override Task<SuperCommand> HandleAsync(SuperCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine("Super");
            return base.HandleAsync(command, cancellationToken);
        }

        public async Task<object> HandleAsync(ICommandExecutionContext commandContext, SuperCommand command)
        {
            await HandleAsync(command, commandContext.CommandAborted);
            return Task.FromResult<object>( null );
        }
    }
}
