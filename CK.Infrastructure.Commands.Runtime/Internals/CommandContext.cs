using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Infrastructure.Commands
{
    internal class CommandContext : ICommandContext
    {
        List<BlobRef> _refs;

        public CommandContext( ICommandRequest request, Guid commandId )
        {
            Request = request;
            CommandId = commandId;
        }

        public Guid CommandId { get; set; }

        public ICommandRequest Request { get; private set; }

        public ICommandResponse Response { get; private set; }

        public Type HandlerType { get; set; }

        public IReadOnlyCollection<BlobRef> Blobs
        {
            get
            {
                if( _refs == null ) return CK.Core.Util.Array.Empty<BlobRef>();
                return _refs;
            }
        }

        public void AddBlobRef( BlobRef blobRef )
        {
            if( blobRef == null ) throw new ArgumentNullException( nameof( blobRef ) );
            if( _refs == null ) _refs = new List<BlobRef>();

            _refs.Add( blobRef );
        }

        internal void CreateDirectResponse( object result )
        {
            if( Response != null ) throw new InvalidOperationException( "There is already a Response created for this CommandContext" );
            Response = new DirectResponse( result, this );
        }

        internal ICommandRunner PrepareHandling( Type handlerType, ICommandHandlerFactory handlerFactoy, ICommandResponseDispatcher dispatcher )
        {
            this.HandlerType = handlerType;
            if( Request.IsLongRunning )
            {
                return new AsyncCommandRunner( handlerFactoy, dispatcher );
            }
            return new CommandRunner( handlerFactoy );
        }

        internal void CreateDeferredResponse()
        {
            if( Response != null ) throw new InvalidOperationException( "There is already a Response created for this CommandContext" );
            Response = new DeferredResponse( Request.CallbackId, this );
        }

        internal void CreateErrorResponse( string msg )
        {
            Response = new ErrorResponse( msg, this );
        }

        internal CommandContext CloneContext()
        {
            return new CommandContext( Request, CommandId )
            {
                HandlerType = this.HandlerType,
                _refs = this._refs
            };
        }
    }

}
