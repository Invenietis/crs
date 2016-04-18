using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs.Pipeline;

namespace CK.Crs.Runtime.Formatting
{
    public class JsonCommandResponseWriter : PipelineComponent
    {
        async public override Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            string jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject( pipeline.Response );
            using( StreamWriter sw = new StreamWriter( pipeline.Output ) ) await sw.WriteAsync( jsonResponse );
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response != null;
        }
    }
}
