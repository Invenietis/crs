using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Formatting
{
    class JsonCommandWriter : PipelineComponent
    {
        readonly IJsonConverter _jsonConverter;
        public JsonCommandWriter( IJsonConverter jsonConverter )
        {
            _jsonConverter = jsonConverter;
        }

        async public override Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            string jsonResponse = _jsonConverter.ToJson( pipeline.Response );
            pipeline.Response.Headers.Add( "Content-Type", "application/json" );

            using( StreamWriter sw = new StreamWriter( pipeline.Output, Encoding.UTF8, 1024, true ) ) await sw.WriteAsync( jsonResponse );
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response != null && pipeline.Output.Length == 0;
        }
    }
}