using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class InMemoryScheduler : IOperationExecutor<ScheduledCommand>, IDisposable
    {
        readonly IList<Timer> _timers;
        readonly ICommandReceiver _schedulerReceiver;

        public InMemoryScheduler( ICommandReceiver schedulerReceiver )
        {
            _schedulerReceiver = schedulerReceiver;

            _timers = new List<Timer>();
        }

        public void Dispose()
        {
            foreach( var t in _timers ) t.Dispose();
        }

        public void Execute( IActivityMonitor monitor, ScheduledCommand operation )
        {
            Timer t = new Timer( async ( state ) =>
            {
                using( var m = operation.Token.CreateDependentMonitor() )
                {
                    var scheduledOperation = state as ScheduledCommand;
                    var commandResponse = await _schedulerReceiver.ProcessCommandAsync( new CommandRequest
                    {
                        Command = scheduledOperation.Payload,
                        CommandDescription = scheduledOperation.Description,
                        CallbackId = operation.CallbackId
                    }, m );
                    
                    // TODO: What to do with the response...
                }

            }, operation, (int) (operation.Scheduling.WhenCommandShouldBeRaised - DateTime.UtcNow ).TotalMilliseconds, Timeout.Infinite);
            _timers.Add( t );
        }
    }
}
