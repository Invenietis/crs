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
            // TODO: why first or default? We must ensure that we get the good one.
            // Maybe by adding a CKTrait during configuration to each RequestDescription. This 
            // traits will identityf the Receiver (which is unique)
            // BTW, We should decouple which request are handle by which receiver...
            // The request registry and the requests by receivers which is a subset of the registry.
            return Requests.FirstOrDefault( t => t.Type == requestType );
        }
    }
}
