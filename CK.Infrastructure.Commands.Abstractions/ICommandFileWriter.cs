using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Framework;

namespace CK.Infrastructure.Commands
{
    public interface ICommandFileWriter
    {
        /// <summary>
        /// Saves the content of a BLOB and its metadata and returns a ref to this BLOB
        /// </summary>
        /// <param name="blobContent"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        Task<BlobRef> SaveAsync( Stream blobContent, IDictionary<string, string> metadata );
    }
}
