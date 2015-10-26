using System;
using System.Diagnostics;
using System.Reflection;

namespace CK.Infrastructure.Commands
{
    public class CommandProcessingContext
    {
        ICommandResponse _response;

        public CommandProcessingContext( Guid commandId, ICommandRequest request  )
        {
            Request = request;
            RuntimeContext = CreateRuntimeContext( commandId, request );
        }

        public ICommandRequest Request { get; private set; }

        public CommandContext RuntimeContext { get; private set; }

        public ICommandResponse CreateDirectResponse( object result )
        {
            if( _response != null ) throw new InvalidOperationException( "There is already a Response created for this CommandContext" );
            _response = new DirectResponse( result, this );
            return _response;
        }

        public ICommandResponse CreateDeferredResponse()
        {
            if( _response != null ) throw new InvalidOperationException( "There is already a Response created for this CommandContext" );
            _response = new DeferredResponse( Request.CallbackId, this );
            return _response;
        }

        public ICommandResponse CreateErrorResponse( string message )
        {
            _response = new ErrorResponse( message, RuntimeContext.CommandId );
            return _response;
        }

        public CommandProcessingContext CloneContext()
        {
            return new CommandProcessingContext( RuntimeContext.CommandId, Request );
        }

        static internal CommandContext CreateRuntimeContext( Guid commandId, ICommandRequest request )
        {
            Type commandContextType = typeof( CommandContext<> ).MakeGenericType( request.CommandDescription.CommandType );
            object instance = Activator.CreateInstance( commandContextType, request.Command, commandId, request.CommandDescription.IsLongRunning, request.CallbackId  );
            return (CommandContext)instance;
        }
    }
}
