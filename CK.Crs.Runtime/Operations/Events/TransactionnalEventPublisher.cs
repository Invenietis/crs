#if NET451
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class TransactionnalEventPublisher : IExternalEventPublisher
    {
        static Func<IOperationExecutor<Event>> _operationExecutorThunk;
        public static void SetEventEmitter( Func<IOperationExecutor<Event>> operationExecutor )
        {
            _operationExecutorThunk = operationExecutor;
        }

        readonly ICommandExecutionContext _context;
        readonly IOperationExecutor<Event> _operationExecutor;
        readonly Lazy<TransactionOperationManager<Event>> _resourceManager;

        public TransactionnalEventPublisher( ICommandExecutionContext context ) : this( context, _operationExecutorThunk() )
        {
        }

        public TransactionnalEventPublisher( ICommandExecutionContext context, IOperationExecutor<Event> operationExecutor )
        {
            _context = context;
            _operationExecutor = operationExecutor;
            _resourceManager = new Lazy<TransactionOperationManager<Event>>(
                () => new TransactionOperationManager<Event>( context.Monitor, operationExecutor ) );
        }

        public void ForcePush<T>( T @event )
        {
            var evt =  CreateEvent( @event );
            PublishEvent( evt );
        }

        public void Push<T>( T @event )
        {
            using( _context.Monitor.OpenTrace().Send( "Event pushed..." ) )
            {
                if( Transaction.Current == null )
                {
                    _context.Monitor.Trace().Send( "No ambient transaction found. Directly publish the event." );
                    ForcePush( @event );
                }
                else
                {
                    _context.Monitor.Trace().Send( "An ambient transaction has been found. Queueing the publishing of the event." );
                    var evt =  CreateEvent( @event );
                    _resourceManager.Value.AddOperation( evt );
                }
            }
        }

        protected virtual Event CreateEvent<T>( T @event )
        {
            var activity = _context.Monitor.DependentActivity();
            return new Event( activity.CreateToken(), @event, typeof( T ) );
        }

        protected virtual void PublishEvent( Event evt )
        {
            _operationExecutor.Execute( _context.Monitor, evt );
        }
    }
}
#endif