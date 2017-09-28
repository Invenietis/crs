using System;

namespace CK.Crs
{
    class MetaCommandResponse : Response<MetaCommand.Result>
    {
        public MetaCommandResponse( MetaCommand.Result result ) : base( Crs.ResponseType.Meta, Guid.Empty.ToString( "N" ) )
        {
            Payload = result;
        }
    }
}
