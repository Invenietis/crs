using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class CommandRejectedContext
    {
        private readonly CommandAction _action;
        private readonly IRejectable _rejectable;

        public CommandRejectedContext( CommandAction action, IRejectable rejectable )
        {
            _action = action;
            _rejectable = rejectable;
        }
        /// <summary>
        /// The command action.
        /// </summary>
        public CommandAction Action => _action;
        /// <summary>
        /// Cancel the rejection of the command. The command will still continue to be processed.
        /// </summary>
        public abstract void CancelRejection();
        /// <summary>
        /// The reason why the command had been rejected.
        /// </summary>
        public string RejecteReason => _rejectable.RejectReason;
    }
}
