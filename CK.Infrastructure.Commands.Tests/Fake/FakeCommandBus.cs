using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands.Tests
{
    internal class FakeCommandBus : ICommandBus
    {
        public IList<ICommand> Commands { get; internal set; }

        public FakeCommandBus()
        {
            Commands = new List<ICommand>();
        }

        public Task<ICommandResult> SendAsync( ICommand command )
        {
            if( command == null ) throw new ArgumentNullException( nameof( command ) );
            Commands.Add( command );
            return Task.FromResult<ICommandResult>( new CommandResult( command ) );
        }
    }
}