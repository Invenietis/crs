using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CK.Core;

namespace CK.Crs.Configuration
{
    class CrsModel : ICrsModel
    {
        List<IEndpointModel> _models;
        CKTraitContext _tagContext;
        public CrsModel( CKTraitContext tagContext )
        {
            _tagContext = tagContext;
            _models = new List<IEndpointModel>();
        }

        public IReadOnlyList<IEndpointModel> Endpoints => _models;

        public CKTraitContext TagContext => _tagContext;

        public IEndpointModel GetEndpoint( string path )
        {
            return _models.SingleOrDefault( t => t.Path == path );
        }

        public void AddEndpoint( IEndpointModel endpoint )
        {
            if( endpoint == null ) throw new ArgumentNullException( nameof( endpoint ) );
            if( _models.Any( x => x.Path.Equals( endpoint.Path, StringComparison.OrdinalIgnoreCase )) )
            {
                throw new ArgumentException( $"A Crs endpoint with the name {endpoint.Path} has already been registered.");
            }
            _models.Add( endpoint );
        }
    }
}
