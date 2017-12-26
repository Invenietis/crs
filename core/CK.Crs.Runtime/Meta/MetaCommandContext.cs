using CK.Core;
using System;
using System.Threading;

namespace CK.Crs.Meta
{
    public sealed class MetaCommandContext : ICommandContext
    {
        public MetaCommandContext( IActivityMonitor monitor, IEndpointModel endpointModel, CancellationToken cancellationToken )
        {
            Monitor = monitor;
            Model = new MetaCommandModel( endpointModel.CrsModel.TraitContext );
            Aborted = cancellationToken;
        }

        public string CommandId { get; } = Guid.Empty.ToString( "N" );

        public IActivityMonitor Monitor { get; }

        public CancellationToken Aborted { get; }

        public CallerId CallerId { get; }

        public CommandModel Model { get; }
    }
}
