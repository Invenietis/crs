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
        /// Gets or sets the result of the command.
        /// </summary>
        public object Result { get; private set; }

        /// <summary>
        /// Gets or sets if there is an <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; private set; }

        public bool IsResponseCreated
        {
            get { return _response != null; }
        }

        public ICommandResponse Response
        {
            get { return _response; }
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

        public void SetResult( object result )
        {
            if( Result != null )
            {
                RuntimeContext.Monitor.Warn().Send( "A Result already exists. It has been overriden..." );
            }
            Result = result;
        }

        /// <summary>
        /// Mutates the context by creating a <see cref="ICommandResponse"/>. 
        /// </summary>
        /// <returns></returns>
        public ICommandResponse CreateResponse()
        {
            if( _response != null )
            {
                if( RuntimeContext.IsLongRunning )
                {
                    return new DirectResponse( Result, RuntimeContext );
                }

                throw new InvalidOperationException( "There is already a Response created for this CommandContext." );
            }

            if( Exception != null ) _response = CreateErrorResponse( Exception.Message );
            else
            {
                if( RuntimeContext.IsLongRunning ) _response = new DeferredResponse( RuntimeContext );
                else _response = new DirectResponse( Result, RuntimeContext );
            }
            return _response;
        }

        public ICommandResponse CreateErrorResponse( string message )
        {
            _response = new ErrorResponse( message, RuntimeContext.CommandId );
            return _response;
        }

        public ICommandResponse CreateInvalidResponse( ICollection<ValidationResult> results )
        {
            _response = new InvalidCommandResponse( results, RuntimeContext );
            return _response;
        }

    }
}