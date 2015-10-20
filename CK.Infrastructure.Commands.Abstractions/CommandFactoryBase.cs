using System;
using System.IO;

namespace CK.Infrastructure.Commands
{
    public abstract class CommandFactoryBase : ICommandFactory
    {
        public ICommand CreateCommand( CommandRouteRegistration routeInfo, Stream requestPayload )
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

        protected abstract ICommand Deserialize( string json, Type t );

        private void AssignCommandIdentifier( ICommand cmd )
        {
            cmd.CommandId = Guid.NewGuid();
        }

    }
}