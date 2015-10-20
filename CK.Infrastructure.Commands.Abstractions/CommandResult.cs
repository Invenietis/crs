using System;

namespace CK.Infrastructure.Commands
{
    public class CommandResult : ICommandResult
    {
        readonly ICommand _command;
        public CommandResult( ICommand command )
        {
            _command = command;
        }

        public Guid CommandId
        {
            get
            {
                return _command.CommandId;
            }
        }

        public bool IsFailure { get; set; }

        public bool IsSuccess { get; set; }

        public object Result { get; set; }
    }
}