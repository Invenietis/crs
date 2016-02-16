using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class CommandRequest : ICommandRequest
    {
        BlobRef[] _refs;

        public CommandRequest( RoutedCommandDescriptor routeInfo )
        {
            CommandDescription = routeInfo;
        }

        public RoutedCommandDescriptor CommandDescription { get; private set; }

        public object Command { get; set; }

        public string CallbackId { get; set; }

        public IReadOnlyCollection<BlobRef> Files
        {
            get
            {
                if( _refs == null ) return new BlobRef[0];
                return _refs;
            }
        }
        
        internal void AddFile( BlobRef blobRef )
        {
            if( blobRef == null ) throw new ArgumentNullException( nameof( blobRef ) );
            if( _refs == null ) _refs = new BlobRef[1]; 
            else Array.Resize( ref _refs, _refs.Length + 1 );

            _refs[_refs.Length - 1] = blobRef;
        }

    }
}
