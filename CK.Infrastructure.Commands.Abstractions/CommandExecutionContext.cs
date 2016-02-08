using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CK.Core;

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

        /// <summary>
        /// Gets the response of the command.
        /// </summary>
        public ICommandResponse Response { get; private set; }

        /// <summary>
        /// Gets or sets if there is an <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; private set; }

        public bool IsResponseCreated
        {
            get { return _response != null; }
        }

        public void SetException( Exception ex )
        {
            if( Exception == null ) Exception = ex;
            else
            {
                RuntimeContext.Monitor.Warn()
                    .Send( "Try to set an Exception of type {0} but the context already has an exception set.", ex.GetType().Name );
            }
        }

        public void SetResponse( ICommandResponse response )
        {
            if( Response != null )
            {
                RuntimeContext.Monitor.Warn().Send( "A Response already exists. It has been overriden..." );
            }
            Response = response;
        }
    }
}