using System;

namespace CK.Crs
{
    class MetaCommandResponse : CommandResponse
    {
        public MetaCommandResponse( MetaCommand.MetaResult result ) : base( CommandResponseType.Meta, Guid.Empty )
        {
            Payload = result;
        }
    }
}
