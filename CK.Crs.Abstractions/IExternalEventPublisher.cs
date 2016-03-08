using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IExternalEventPublisher
    {
        void Push<T>( T @event );
        void ForcePush<T>( T @event );
    }
}
