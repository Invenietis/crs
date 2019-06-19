using CK.Core;
using System;
using System.Collections.Generic;

namespace CK.Crs
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICrsModel
    {
        /// <summary>
        /// Gets the <see cref="CKTraitContext"/> associated to this endpoint
        /// </summary>
        CKTraitContext TraitContext { get; }

        IReadOnlyList<IEndpointModel> Endpoints { get; }

        void AddEndpoint( IEndpointModel model );
    }
}
