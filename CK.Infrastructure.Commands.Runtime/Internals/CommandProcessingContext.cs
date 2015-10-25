using System;

namespace CK.Infrastructure.Commands
{
    internal class CommandProcessingContext
    {
        ICommandResponse _response;

        public CommandProcessingContext( ICommandRequest request, Guid commandId )
        {
            Request = request;
            CommandId = commandId;
        }

        public Guid CommandId { get; set; }

        public ICommandRequest Request { get; private set; }

        public Type HandlerType { get; set; }

        internal ICommandResponse CreateDirectResponse( object result )
        {
            if( _response != null ) throw new InvalidOperationException( "There is already a Response created for this CommandContext" );
            _response = new DirectResponse( result, this );
            return _response;
        }

        internal ICommandResponse CreateDeferredResponse()
        {
            if( _response != null ) throw new InvalidOperationException( "There is already a Response created for this CommandContext" );
            _response = new DeferredResponse( Request.CallbackId, this );
            return _response;
        }

        internal ICommandResponse CreateErrorResponse( string msg )
        {
            _response = new ErrorResponse( msg, this );
            return _response;
        }

        internal CommandProcessingContext CloneContext()
        {
            return new CommandProcessingContext( Request, CommandId )
            {
                HandlerType = this.HandlerType
            };
        }
    }

}
