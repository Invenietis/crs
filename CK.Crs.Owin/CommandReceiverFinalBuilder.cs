using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public sealed class CommandReceiverFinalBuilder : CommandReceiverBuilder
    {
        protected override void ApplyDefaultConfiguration( CommandReceiverConfiguration config )
        {
            base.ApplyDefaultConfiguration( config );
            config.Pipeline.UseDefault().UseSyncCommandExecutor().UseJsonResponseWriter();
        }
    }
}
