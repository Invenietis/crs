using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CK.Infrastructure.Commands;
using Newtonsoft.Json;

namespace CK.Infrastructure.Commands
{
    internal class DefaultCommandFactory : ICommandFactory
    {
        public object CreateCommand( CommandRouteRegistration routeInfo, Stream requestPayload )
        {
            if( !typeof( ICommand ).IsAssignableFrom( routeInfo.CommandType ) )
            {
                throw new ArgumentException( "The command type must implement ICommand." );
            }

            string json = ReadBody( requestPayload );
            if( String.IsNullOrEmpty( json ) )
            {
                throw new InvalidOperationException( "The request is empty." );
            }

            ICommand cmd = Deserialize( json, routeInfo.CommandType );
            AssignCommandIdentifier( cmd );
            return cmd;
        }

        protected virtual string ReadBody( Stream requestPayload )
        {
            // TODO: better reads the stream
            using( var reader = new StreamReader( requestPayload ) )
            {
                return reader.ReadToEnd();
            }
        }

        protected virtual ICommand Deserialize( string json, Type t )
        {
            ICommand command = (ICommand)JsonConvert.DeserializeObject( json, t );
            Debug.Assert( command != null );
            return command;
        }

        private void AssignCommandIdentifier( ICommand cmd )
        {
            cmd.CommandId = Guid.NewGuid();
        }

    }
}