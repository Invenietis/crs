using System;

namespace CK.Crs
{
    class MetaCommandResponse : Response
    {
        public MetaCommandResponse( MetaCommand.Result result ) : base( Crs.ResponseType.Meta, Guid.Empty )
        {
            Payload = result;
        }
    }
}
