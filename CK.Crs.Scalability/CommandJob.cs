using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability
{
    public sealed class CommandJob : IDisposable
    {
        readonly IDisposable _jobActivity;
        public CommandJob( CommandJobMetada metaData, byte[] payload )
        {
            var m = new ActivityMonitor();
            _jobActivity = m.StartDependentActivity( ActivityMonitor.DependentToken.Parse( metaData.MonitorToken) );
            Monitor = m; 
            MetaData = metaData;
            Payload = payload;
        }

        public IActivityMonitor Monitor { get; }

        public byte[] Payload { get; }

        public CommandJobMetada MetaData { get; }

        public void Dispose()
        {
            _jobActivity.Dispose();
        }
    }
}
