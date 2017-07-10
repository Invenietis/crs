using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public interface ICrsModel
    {
        IReadOnlyList<ICrsEndpointModel> Endpoints { get; }
    }
}
