using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CommandRequest 
    {
        /// <summary>
        /// Command description
        /// </summary>
        public RoutedCommandDescriptor CommandDescription { get;  set; }

        /// <summary>
        /// The instance of the command to process. This should never be null.
        /// </summary>
        public object Command { get; set; }

        /// <summary>
        /// Returns an identifier that should help identifying the caller of this request.
        /// </summary>
        public string CallbackId { get; set; }

    }
}
