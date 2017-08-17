using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CK.Crs.Infrastructure
{
    class CrsModel : ICrsModel
    {
        List<IEndpointModel> _models;
        public CrsModel()
        {
            _models = new List<IEndpointModel>();
        }
        public IReadOnlyList<IEndpointModel> Endpoints => _models;

        public IEndpointModel GetEndpoint( Type type )
        {
            // TODO: lookup in a dictionary ?
            return _models.SingleOrDefault( t => t.EndpointType == type.GetGenericTypeDefinition() );
        }

        internal void AddEndpoint( CrsReceiverModel endpoint )
        {
            if( endpoint == null ) throw new ArgumentNullException( nameof( endpoint ) );
            if( _models.Any( x => x.Name.Equals( endpoint.Name, StringComparison.OrdinalIgnoreCase )) )
            {
                throw new ArgumentException( $"A Crs endpoint with the name {endpoint.Name} has already been registered.");
            }
            _models.Add( endpoint );
        }
    }
}
