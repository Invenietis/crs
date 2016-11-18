using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Crs.Runtime;

namespace CK.Crs.OpenAPI
{
    public interface ISwaggerProvider
    {
        SwaggerDocument GetSwagger( ICrsConfiguration configuration, string host = null, string[] schemes = null );
    }

    public class UnknownSwaggerDocument : Exception
    {
        public UnknownSwaggerDocument( string documentName )
            : base( string.Format( "Unknown Swagger document - {0}", documentName ) )
        {
        }
    }
}
