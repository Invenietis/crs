using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICrsModel
    {
        IReadOnlyList<IEndpointModel> Endpoints { get; }

        IEndpointModel GetEndpoint( Type type );
    }
}
