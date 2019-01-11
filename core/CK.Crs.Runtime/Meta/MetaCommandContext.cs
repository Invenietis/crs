using CK.Core;
using System;
using System.Threading;

namespace CK.Crs.Meta
{
    public sealed class MetaCommandContext : CommandContext
    {
        public MetaCommandContext( IActivityMonitor monitor, IEndpointModel endpointModel, CancellationToken cancellationToken )
            : base( Guid.Empty.ToString( "N" ), monitor, new MetaCommandModel( endpointModel.CrsModel.TraitContext ),endpointModel ,CallerId.None, cancellationToken )
        {
        }
    }
}
