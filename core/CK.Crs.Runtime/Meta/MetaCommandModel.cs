using CK.Core;

namespace CK.Crs
{
    class MetaCommandModel : CommandModel
    {
        public MetaCommandModel( CKTraitContext context ) : base( typeof( MetaCommand ), context )
        {
        }
    }
}
