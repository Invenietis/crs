using CK.Core;
using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICrsModel
    {

        /// <summary>
        /// Gets the <see cref="CKTraitContext"/> associated to this endpoint
        /// </summary>
        CKTraitContext TraitContext { get; }

        IReadOnlyList<IEndpointModel> Endpoints { get; }

        IEndpointModel GetEndpoint( Type type );
    }
}
