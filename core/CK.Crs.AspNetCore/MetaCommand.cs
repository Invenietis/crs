using CK.Crs.Infrastructure;
using System.Collections.Generic;
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

    class MetaCommandModel : CommandModel
    {
        public MetaCommandModel( CKTraitContext context ) : base( typeof( MetaCommand ), context )
        {
        }
    }

    public class MetaCommand : ICommand<MetaCommand.Result>
    {
        public bool ShowCommands { get; set; }
        public bool ShowAmbientValues { get; set; }
        public class Result
        {
            public int Version { get; set; }
            public IDictionary<string, object> AmbientValues { get; set; }
            public Dictionary<string, MetaCommandDescription> Commands { get; set; }

            public class MetaCommandDescription
            {
                public string CommandType { get; set; }
                public string CommandName { get; set; }
                public string ResultType { get; set; }
                public string Traits { get; set; }
                public string Description { get; set; }
                public RequestPropertyInfo[] Parameters { get; set; }
            }
        }
    }
}
