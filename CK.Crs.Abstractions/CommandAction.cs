using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CommandAction
    {
        public CommandAction( Guid commandId )
        {
            CommandId = commandId;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the unique command identifier.
        /// </summary>
        public Guid CommandId { get; }

        /// <summary>
        /// The instance of the command to process. This should never be null.
        /// </summary>
        public object Command { get; set; }

        /// <summary>
        /// Command description
        /// </summary>
        public CommandRoute Description { get; set; }

        /// <summary>
        /// Returns an identifier that should help identifying the caller of this request.
        /// </summary>
        public string CallbackId { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> this command action has been submitted to the receiver.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}
