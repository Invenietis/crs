using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Infrastructure
{

    class CrsEndpointModel : ICrsEndpointModel
    {
        public string Name => EndpointType.Name;

        public Type EndpointType { get; internal set; }

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        public bool ValidateAmbientValues { get; internal set; } = true;
        public bool ValidateModel { get; internal set; } = true;

        public IReadOnlyList<RequestDescription> Requests { get; internal set; }

        public RequestDescription GetRequestDescription( Type requestType )
        {
            // TODO: lookup in a dictionary ?
            return Requests.SingleOrDefault( t => t.Type == requestType );
        }
    }
}
