using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Infrastructure
{

    class CrsReceiverModel : ICrsReceiverModel
    {
        private Type _currentConfiguredEndpoint;
        private IEnumerable<RequestDescription> _requests;

        public CrsReceiverModel( Type currentConfiguredEndpoint, IEnumerable<RequestDescription> requests )
        {
            _currentConfiguredEndpoint = currentConfiguredEndpoint;
            _requests = requests;
        }

        public string Name => _currentConfiguredEndpoint.Name;

        public Type ReceiverType => _currentConfiguredEndpoint;

        public string CallerIdName { get; set; } = "CallerId";

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        public bool ValidateAmbientValues { get; internal set; } = true;

        public bool ValidateModel { get; internal set; } = true;

        public bool SupportsClientEventsFiltering => ReflectionUtil.IsAssignableToGenericType( ReceiverType, typeof( ICrsListener ) );

        public IEnumerable<RequestDescription> Requests => _requests;

        public RequestDescription GetRequestDescription( Type requestType )
        {
            // TODO: lookup in a dictionary ?
            return Requests.SingleOrDefault( t => t.Type == requestType );
        }
    }
}
