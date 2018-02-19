using System;

namespace CK.Crs.Meta
{
    public sealed class MetaCommandResponse : Response<MetaCommand.Result>
    {
        public MetaCommandResponse( MetaCommand.Result result ) : base( Crs.ResponseType.Meta, Guid.Empty.ToString( "N" ), result )
        {
        }
    }
}
