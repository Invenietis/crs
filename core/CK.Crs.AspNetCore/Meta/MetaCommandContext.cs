using CK.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;

namespace CK.Crs
{
    sealed class MetaCommandContext : IHttpCommandContext
    {
        readonly HttpContext _httpContext;
        public MetaCommandContext( IActivityMonitor monitor, IEndpointModel endpointModel, HttpContext httpContext, CancellationToken cancellationToken )
        {
            Monitor = monitor;
            Model = new MetaCommandModel( endpointModel.CrsModel.TraitContext );
            _httpContext = httpContext;
            Aborted = cancellationToken;
        }

        public string CommandId { get; } = Guid.Empty.ToString( "N" );

        public IActivityMonitor Monitor { get; }

        public CancellationToken Aborted { get; }

        public CallerId CallerId { get; }

        public CommandModel Model { get; }

        public HttpContext GetHttpContext() => _httpContext;
    }
}
