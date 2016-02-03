using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    class CommandRegistration<T> : ICommandRegistration
    {
        CommandDescriptor _commandDescription;
        
        public CommandRegistration( CommandDescriptor commandDescription )
        {
            _commandDescription = commandDescription;
        }

        public ICommandRegistration HandledBy<THandler>() where THandler : ICommandHandler
        {
            _commandDescription.HandlerType = typeof( THandler );
            return this;
        }

        public ICommandRegistration AddDecorator<TDecorator>() where TDecorator : ICommandDecorator
        {
            _commandDescription.Decorators = _commandDescription.Decorators.Union( new[] { typeof( TDecorator ) } ).ToArray();
            return this;
        }

        public ICommandRegistration CommandName( string commandName )
        {
            _commandDescription.Name = commandName;
            return this;
        }

        public ICommandRegistration IsLongRunning()
        {
            _commandDescription.IsLongRunning = true;
            return this;
        }
    }

}
