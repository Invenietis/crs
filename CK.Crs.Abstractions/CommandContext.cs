using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CommandContext
    {
        public CommandContext( CommandDescriptor commandDescription, Command command )
        {
            if( commandDescription == null ) throw new ArgumentNullException( "commandDescription" );
            if( command == null ) throw new ArgumentNullException( "commandContext" );

            Description = commandDescription;
            Command = command;
            Items = new Dictionary<object, object>();
        }

        public Command Command { get; private set; }

        public CommandDescriptor Description { get; private set; }

        /// <summary>
        /// Gets a bag of data that can hold data during the command execution
        /// </summary>
        public IDictionary<object, object> Items { get; private set; }

        /// <summary>
        /// Gets the result of the command.
        /// </summary>
        public object Result { get; private set; }

        /// <summary>
        /// Gets or sets if there is an <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; private set; }

        public bool IsDirty
        {
            get { return Result != null || Exception != null; }
        }

        public void SetException( Exception ex )
        {
            if( Exception == null )
            {
                Exception = ex;
            }
            else
            {
                Command.Monitor.Warn().Send( "Try to set an Exception of type {0} but the context already has an exception set.", ex.GetType().Name );
            }
        }

        public void SetResult( object result )
        {
            if( Result != null )
            {
                Command.Monitor.Warn().Send( "A Result already exists. It has been overriden..." );
            }
            Result = result;
        }
    }
}
