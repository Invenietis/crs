using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Infrastructure
{

    class CrsReceiverModel : ICrsReceiverModel
    {
        public string Name => ReceiverType.Name;

        public Type ReceiverType { get; internal set; }

        public string CallerIdName { get; set; } = "CallerId";

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        public bool ValidateAmbientValues { get; internal set; } = true;

        public bool ValidateModel { get; internal set; } = true;

        public bool SupportsClientEventsFiltering => ReflectionUtil.IsAssignableToGenericType( ReceiverType, typeof( ICrsListener ) );

        public IReadOnlyList<RequestDescription> Requests { get; internal set; }

        public RequestDescription GetRequestDescription( Type requestType )
        {
            // TODO: lookup in a dictionary ?
            return Requests.SingleOrDefault( t => t.Type == requestType );
        }
    }
}
