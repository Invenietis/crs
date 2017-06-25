using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability
{
    public sealed class CommandJob
    {
        public CommandJob( IActivityMonitor monitor, CommandJobMetada metaData, byte[] payload )
        {
            Monitor = monitor;
            MetaData = metaData;
            Payload = payload;
        }

        public IActivityMonitor Monitor { get; }

        public byte[] Payload { get; }

        public CommandJobMetada MetaData { get; }
    }
}
