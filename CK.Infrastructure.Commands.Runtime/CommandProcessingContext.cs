using System;
using System.Diagnostics;
using System.Reflection;

namespace CK.Infrastructure.Commands
{
    internal class CommandProcessingContext
    {
        private readonly ICommandRequest _request;

        public CommandProcessingContext( Guid commandId, ICommandRequest request )
        {
            _request = request;
        }

        public CommandContext RuntimeContext { get; private set; }

        public CommandExecutionContext ExecutionContext { get; private set; }

    }
}
