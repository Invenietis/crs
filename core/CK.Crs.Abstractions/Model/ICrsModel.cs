using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CK.Crs
{
    public interface ICrsModel
    {
        IReadOnlyList<ICrsEndpointModel> Endpoints { get; }

        ICrsEndpointModel GetEndpoint( TypeInfo type );
    }
}
