using CK.Core;
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
        CKTraitContext TagContext { get; }

        IReadOnlyList<IEndpointModel> Endpoints { get; }

        void AddEndpoint( IEndpointModel model );
    }
}
