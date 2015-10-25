using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandFileStore : ICommandFileWriter
    {
        Dictionary<BlobRef, Blob> Memory { get; } = new Dictionary<BlobRef, Blob>();

        public async Task<BlobRef> SaveAsync( Stream blobContent, string contentType, string contentDisposition, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            string blobId = Guid.NewGuid().ToString("N");
            using( MemoryStream ms = new MemoryStream() )
            {
                await blobContent.CopyToAsync( ms );
                var key = new BlobRef { BlobRefId = blobId };
                Memory.Add( key, new Blob
                {
                    Metadata = new Dictionary<string, string> { { "ContentType", contentType }, { "ContentDisposition", contentDisposition } },
                    RawContent = ms.ToArray()
                } );

                return key;
            }
        }

        class Blob
        {
            public byte[] RawContent { get; set; }
            public IDictionary<string, string> Metadata { get; set; }
        }
    }
}
