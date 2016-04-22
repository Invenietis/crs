using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;

namespace CK.Crs.Runtime.Meta
{
    public class MetaComponent : PipelineComponent
    {
        readonly IAmbientValues _ambientValues;
        readonly IAmbientValuesRegistration _registration;
        readonly IMemoryCache _cache;

        public MetaComponent( IAmbientValues ambientValues, IAmbientValuesRegistration registration, IMemoryCache cache )
        {
            _ambientValues = ambientValues;
            _registration = registration;
            _cache = cache;
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Request.Path.CommandName == "__meta";
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            using( StreamReader sr = new StreamReader( pipeline.Request.Body ) )
            {
                var result = new MetaCommand.MetaResult();
                var command = Newtonsoft.Json.JsonConvert.DeserializeObject<MetaCommand>( sr.ReadToEnd() ) ?? new MetaCommand { ShowAmbientValues = true, ShowCommands = true };
                if( command.ShowAmbientValues )
                {
                    result.AmbientValues = new Dictionary<string, object>();
                    foreach( var a in _registration.AmbientValues )
                    {
                        if( _ambientValues.IsDefined( a.Name ) )
                        {
                            var o = await _ambientValues.GetValueAsync<object>( a.Name );
                            result.AmbientValues.Add( a.Name, o );
                        }
                    }
                }
                if( command.ShowCommands )
                {
                    result.Commands = new Dictionary<string, MetaCommand.MetaResult.MetaCommandDescription>();
                    foreach( var c in pipeline.Configuration.Routes )
                    {
                        MetaCommand.MetaResult.MetaCommandDescription desc;
                        if( !_cache.TryGetValue( c.Key.CommandName, out desc ) )
                        {
                            desc = new MetaCommand.MetaResult.MetaCommandDescription
                            {
                                Route = c.Value.Route,
                                CommandType = c.Value.Descriptor.CommandType.AssemblyQualifiedName,
                                Parameters = c.Value.Descriptor.CommandType.GetTypeInfo().DeclaredProperties.Select( e => new CommandPropertyInfo( e, _registration ) ).ToArray(),
                                Traits = c.Value.Descriptor.Traits,
                                Description = c.Value.Descriptor.Description
                            };
                            _cache.Set( c.Key.CommandName, desc );
                        }
                        result.Commands.Add( c.Key.CommandName, desc );
                    }
                }

                pipeline.Response = new MetaCommandResponse( result );
            }
        }
    }

    class MetaCommandResponse : CommandResponse
    {
        public MetaCommandResponse( MetaCommand.MetaResult result ) : base( CommandResponseType.Meta, Guid.Empty )
        {
            Payload = result;
        }
    }

    class MetaCommand
    {
        public bool ShowCommands { get; set; }
        public bool ShowAmbientValues { get; set; }
        public class MetaResult
        {
            public int Version { get; set; }
            public IDictionary<string, object> AmbientValues { get; set; }
            public Dictionary<string, MetaCommandDescription> Commands { get; set; }
            public class MetaCommandDescription
            {
                public string CommandType { get; internal set; }
                public CommandRoutePath Route { get; internal set; }
                public string Traits { get; internal set; }
                public string Description { get; internal set; }
                public CommandPropertyInfo[] Parameters { get; internal set; }
            }
        }
    }
}
