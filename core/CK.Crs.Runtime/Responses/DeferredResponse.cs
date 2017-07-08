using CK.Crs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{

    /// <summary>
    /// Represent the A of VISAM : Asynchronous response.
    /// </summary>
    public class DeferredResponse : Response
    {
        public DeferredResponse(Guid id, string callbackId) : base(Crs.ResponseType.Asynchronous, id)
        {
            Payload = callbackId;
        }
    }

}
