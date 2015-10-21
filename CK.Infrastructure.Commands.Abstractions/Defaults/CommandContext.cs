using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Framework;

namespace CK.Infrastructure.Commands
{
    public class CommandContext : ICommandContext
    {
        List<BlobRef> _refs;

        public CommandContext( IEventDispatcher eventDispatcher, ICommandHandlerFactory handlerFactory )
        {
            EventDispatcher = eventDispatcher;
            HandlerFactory = handlerFactory;
        }

        public ICommandRequest Request { get; set; }

        public ICommandResponse Response { get; set; }

        public IEventDispatcher EventDispatcher { get; private set; }

        public ICommandHandlerFactory HandlerFactory { get; private set; }

        public Type HandlerType { get; set; }

        public bool IsAsynchronous { get; set; }

        public IReadOnlyCollection<BlobRef> Blobs
        {
            get
            {
                if( _refs == null ) return CK.Core.Util.EmptyArray<BlobRef>.Empty;
                return _refs;
            }
        }

        public void AddBlobRef( BlobRef blobRef )
        {
            if( blobRef == null ) throw new ArgumentNullException( nameof( blobRef ) );
            if( _refs == null ) _refs = new List<BlobRef>();

            _refs.Add( blobRef );
        }
    }

}
