using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Framework;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandFileStore : ICommandFileWriter
    {
        Dictionary<BlobRef, Blob> Memory { get; } = new Dictionary<BlobRef, Blob>();

        public async Task<BlobRef> SaveAsync( Stream blobContent, IDictionary<string, string> metadata )
        {
            string blobId = Guid.NewGuid().ToString("N");
            using( MemoryStream ms = new MemoryStream() )
            {
                await blobContent.CopyToAsync( ms );
                var key = new BlobRef { BlobRefId = blobId };
                Memory.Add( key, new Blob
                {
                    Metadata = metadata,
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
