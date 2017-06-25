using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Scalability.Azure
{
    internal class AzureQueueDispatcher : ICommandResponseDispatcher
    {
        private AzureQueueConfiguration conf;

        public AzureQueueDispatcher(AzureQueueConfiguration conf)
        {
            this.conf = conf;
        }

        public Task DispatchAsync(IActivityMonitor monitor, string callbackId, CommandResponseBuilder response, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}