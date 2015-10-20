using System;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandBus : ICommandBus
    {
        public DefaultCommandBus()
        {
        }

        public Task<ICommandResult> SendAsync( ICommand command )
        {
            return Task.FromResult<ICommandResult>( new CommandResult( command ) );
        }
    }
}