using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Formatting
{
    class JsonCommandWriter : PipelineComponent
    {
        readonly IJsonConverter _jsonConverter;
        public JsonCommandWriter( IJsonConverter jsonConverter )
        {
            _jsonConverter = jsonConverter;
        }

        public override Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            pipeline.Response.Headers.Add( "Content-Type", "application/json" );
            pipeline.Response.RegisterOutputHandler( async ( response, output ) =>
            {
                string jsonResponse = _jsonConverter.ToJson(response);
                using (StreamWriter sw = new StreamWriter(output, Encoding.UTF8, 8096, true))
                    await sw.WriteAsync(jsonResponse);
            });

            return Task.CompletedTask;
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response.HasReponse;
        }
    }
}