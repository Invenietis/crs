using CK.Crs;
using CK.Crs.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsQueueExtensions
    {
        public static ICrsCoreBuilder AddInMemoryQueue( this ICrsCoreBuilder builder )
        {
            builder.Services.AddSingleton<InMemoryQueue>();
            builder.AddReceiver<QueueCommandReceiver>();
            return builder;
        }
    }
}
