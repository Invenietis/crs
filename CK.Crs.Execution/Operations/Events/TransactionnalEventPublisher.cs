#if NET451
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CK.Core;

namespace CK.Crs.Runtime.Execution
{
    public class TransactionnalEventPublisher : IExternalEventPublisher
    {
        readonly IOperationExecutor<Event> _operationExecutor;
        public TransactionnalEventPublisher( IOperationExecutor<Event> operationExecutor )
        {
            _operationExecutor = operationExecutor;
        }

        public void ForcePush<T>( IActivityMonitor monitor, T @event )
        {
            var evt =  CreateEvent( monitor, @event );
            PublishEvent( monitor, evt );
        }

        public void Push<T>( IActivityMonitor monitor, T @event )
        {
            using( monitor.OpenTrace().Send( "Event pushed..." ) )
            {
                if( Transaction.Current == null )
                {
                    monitor.Trace().Send( "No ambient transaction found. Directly publish the event." );
                    ForcePush( monitor, @event );
                }
                else
                {
                    monitor.Trace().Send( "An ambient transaction has been found. Queueing the publishing of the event." );
                    var evt =  CreateEvent( monitor, @event );

                    TransactionOperationManager<Event>.AddOperation( monitor, _operationExecutor, evt );
                }
            }
        }

        protected virtual Event CreateEvent<T>( IActivityMonitor monitor, T @event )
        {
            var activity = monitor.DependentActivity();
            return new Event( activity.CreateToken(), @event, typeof( T ) );
        }

        protected virtual void PublishEvent( IActivityMonitor monitor, Event evt )
        {
            _operationExecutor.Execute( monitor, evt );
        }
    }
}
#endif