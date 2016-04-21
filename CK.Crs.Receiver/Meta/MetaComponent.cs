using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Meta
{
    public class MetaComponent : PipelineComponent
    {
        readonly IAmbientValues _ambientValues;
        readonly IAmbientValuesRegistration _registration;
        public MetaComponent( IAmbientValues ambientValues, IAmbientValuesRegistration registration )
        {
            _ambientValues = ambientValues;
            _registration = registration;
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Request.Path.CommandName == "__meta";
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            using( StreamReader sr = new StreamReader( pipeline.Request.Body ) )
            {
                var result = new MetaResult();
                var command = Newtonsoft.Json.JsonConvert.DeserializeObject<MetaCommand>( sr.ReadToEnd() );
                if( command.ShowAmbientValues )
                {
                    result.AmbientValues = new Dictionary<string, object>();
                    foreach( var a in _registration.AmbientValues )
                    {
                        if( _ambientValues.IsDefined( a.Name ) ) result.AmbientValues.Add( a.Name, await _ambientValues.GetValueAsync<object>( a.Name ) );
                    }
                }
                // TODO: show documentation

                pipeline.Response = new MetaCommandResponse( result );
            }
        }
    }

    class MetaCommandResponse : CommandResponse
    {
        public MetaCommandResponse( MetaResult result ) : base( CommandResponseType.Meta, Guid.Empty )
        {
            Payload = result;
        }
    }

    class MetaResult
    {
        public int Version { get; set; }
        public IDictionary<string, object> AmbientValues { get; set; }
    }

    class MetaCommand
    {
        public bool ShowCommands { get; set; }

        public bool ShowAmbientValues { get; set; }
    }
}
