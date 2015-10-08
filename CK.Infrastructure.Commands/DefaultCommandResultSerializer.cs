using System;
using System.IO;
using System.Threading.Tasks;
using CK.Infrastructure.Commands;

namespace CK.Infrastructure.Commands
{
    internal class DefaultCommandResultSerializer : ICommandResultSerializer
    {
        public Task SerializeCommandResult( ICommandResult result, Stream outputStream )
        {
            using( StreamWriter sw = new StreamWriter( outputStream ) )
            {
                return sw.WriteAsync( Newtonsoft.Json.JsonConvert.SerializeObject( result ) );
            }
        }
    }
}