using System;
using System.Collections.Generic;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// The command execution context
    /// </summary>
    public class CommandExecutionContext
    {
        private ICommandResponse _response;

        public CommandExecutionContext( CommandDescriptor commandDescription, CommandContext commandContext )
        {
            if( commandDescription == null ) throw new ArgumentNullException( "commandDescription" );
            if( commandContext == null ) throw new ArgumentNullException( "commandContext" );

            CommandDescription = commandDescription;
            RuntimeContext = commandContext;
            Items = new Dictionary<object, object>();
        }

        public CommandDescriptor CommandDescription { get; private set; }

        public CommandContext RuntimeContext { get; private set; }

        /// <summary>
        /// Gets a bag of data that can hold data during the command execution
        /// </summary>
        public IDictionary<object, object> Items { get; private set; }

        public ICommandResponse CreateResponse()
        {
            if( _response != null ) throw new InvalidOperationException( "There is already a Response created for this CommandContext" );
            if( Exception != null )
            {
                _response = CreateErrorResponse( Exception.Message );
            }
            else
            {
                if( RuntimeContext.IsLongRunning ) _response = new DeferredResponse( RuntimeContext.CallbackId, RuntimeContext );
                else _response = new DirectResponse( Result, RuntimeContext );
            }
            return _response;
        }

        public ICommandResponse CreateErrorResponse( string message )
        {
            _response = new ErrorResponse( message, RuntimeContext.CommandId );
            return _response;
        }

        /// <summary>
        /// Gets or sets the result of the command.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets if there is an <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; set; }
    }
}