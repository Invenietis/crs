using System;
using System.Threading;

namespace CK.Crs.Scalability.Azure
{
    internal class AzureQueueListener : ICommandListener
    {
        private AzureQueueConfiguration conf;

        public AzureQueueListener(AzureQueueConfiguration conf)
        {
            this.conf = conf;
        }

        public void Receive(ICommandWorkerQueue workerQueue, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}