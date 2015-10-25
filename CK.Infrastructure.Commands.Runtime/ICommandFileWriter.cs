using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandFileWriter
    {
        /// <summary>
        /// Saves the content of a BLOB and its metadata and returns a ref to this BLOB
        /// </summary>
        /// <param name="blobContent"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<BlobRef> SaveAsync( Stream blobContent, string contentType, string contentDisposition, CancellationToken cancellationToken = default( CancellationToken ) );
    }
}
