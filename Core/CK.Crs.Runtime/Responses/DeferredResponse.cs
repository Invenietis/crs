using CK.Crs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Responses
{

    /// <summary>
    /// Represent the A of VISAM : Asynchronous response.
    /// </summary>
    public class DeferredResponse : Response<string>
    {
        public DeferredResponse( string commandId, CallerId callerId ) : base( Crs.ResponseType.Asynchronous, commandId, callerId.ToString() )
        {
        }
    }

}
