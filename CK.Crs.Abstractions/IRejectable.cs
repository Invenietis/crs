using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IRejectable
    {
        /// <summary>
        /// Canceled
        /// </summary>
        bool Rejected { get; }

        /// <summary>
        /// 
        /// </summary>
        string RejectReason { get; }

        /// <summary>
        /// Rejects with the given reason.
        /// </summary>
        /// <param name="reason"></param>
        void Reject( string reason );
    }
}
