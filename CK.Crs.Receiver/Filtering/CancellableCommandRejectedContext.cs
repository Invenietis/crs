using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Filtering
{
    class CancellableCommandRejectedContext : CommandRejectedContext
    {
        /// <summary>
        /// Gets wether the command rejection has been canceled or not.
        /// </summary>
        public bool Canceled { get; private set; }

        public CancellableCommandRejectedContext( CommandAction action, IRejectable rejectable ) : base( action, rejectable )
        {
        }

        public override void CancelRejection()
        {
            Canceled = true;
        }
    }
}
