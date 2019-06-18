using CK.Core;

namespace CK.Crs.Meta
{
    class MetaCommandModel : CommandModel
    {
        public MetaCommandModel( CKTraitContext context ) : base( typeof( MetaCommand ), context )
        {
        }
    }
}
