using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandExecutionContext
    {
        Guid CommandId { get; }
        string CallbackId { get; }
        IActivityMonitor Monitor { get; }
        CancellationToken CommandAborted { get; }

        Task PublishEventAsync<T>( T @event );
    }
}