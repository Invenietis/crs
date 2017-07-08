using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CK.Crs
{

    public interface IBus : ICommandDispatcher, IEventPublisher { }
}
