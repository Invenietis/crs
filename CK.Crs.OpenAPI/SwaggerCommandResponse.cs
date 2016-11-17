using System;
using CK.Crs.Runtime;

namespace CK.Crs.OpenAPI
{
    internal class SwaggerCommandResponse : CommandResponse
    {
        public SwaggerCommandResponse( CommandResponseType meta, SwaggerDocument swaggerDocument ) : base( meta, Guid.Empty )
        {
            this.Payload = swaggerDocument;
        }
    }
}