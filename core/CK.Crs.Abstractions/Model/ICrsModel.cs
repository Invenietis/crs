using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICrsModel
    {
        IReadOnlyList<ICrsReceiverModel> Receivers { get; }

        ICrsReceiverModel GetReceiver( Type type );
    }
}
