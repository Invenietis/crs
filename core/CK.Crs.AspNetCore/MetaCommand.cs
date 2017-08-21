using CK.Crs.Infrastructure;
using System.Collections.Generic;
using CK.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;

namespace CK.Crs
{
    public class MetaCommandContext : IHttpCommandContext
    {
        readonly HttpContext _httpContext;
        public MetaCommandContext( IActivityMonitor monitor, IEndpointModel endpointModel, HttpContext httpContext, CancellationToken cancellationToken )
        {
            Monitor = monitor;
            Model = new MetaCommandModel( endpointModel.CrsModel.TraitContext );
            _httpContext = httpContext;
            Aborted = cancellationToken;
        }

        public Guid CommandId => Guid.Empty;

        public IActivityMonitor Monitor { get; }

        public CancellationToken Aborted  { get; }

        public string CallerId => null;

        public CommandModel Model { get; }

        public HttpContext GetHttpContext() => _httpContext;
    }

    class MetaCommandModel : CommandModel
    {
        public MetaCommandModel( CKTraitContext context ) : base( typeof( MetaCommand ), context )
        {
        }
    }

    class MetaCommand
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
                public string CommandType { get; internal set; }
                public string CommandName { get; internal set; }
                public string ResultType { get; internal set; }
                public string Traits { get; internal set; }
                public string Description { get; internal set; }
                public RequestPropertyInfo[] Parameters { get; internal set; }
            }
        }
    }
}
