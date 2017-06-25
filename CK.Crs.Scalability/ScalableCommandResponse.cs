using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability
{
    public class ScalableCommandResponse : CommandResponse
    {
        public ScalableCommandResponse( CommandAction action ) : base(CommandResponseType.Asynchronous,action.CommandId)
        {
            Payload = action.CallbackId;
        }
    }
}
