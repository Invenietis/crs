using System;
using System.Collections.Generic;
using System.Linq;

namespace CK.Crs.Infrastructure
{
    class CrsModel : ICrsModel
    {
        List<ICrsEndpointModel> _models;
        public CrsModel()
        {
            _models = new List<ICrsEndpointModel>();
        }
        public IReadOnlyList<ICrsEndpointModel> Endpoints => _models;

        internal void AddEndpoint( CrsEndpointModel endpoint )
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
