using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class TransactionnalEventPublisher :  IExternalEventPublisher, IEnlistmentNotification
    {
        static Func<Event, Task> _delegate;

        bool _enlisted;
        readonly Queue<Event> _unCommitedEvents;
        readonly ICommandExecutionContext _context;

        public TransactionnalEventPublisher( ICommandExecutionContext context )
        {
            _context = context;
            _unCommitedEvents = new Queue<Event>();
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
                    if( !_enlisted )
                    {
                        Transaction.Current.EnlistVolatile( this, EnlistmentOptions.None );
                        _enlisted = true;
                        _context.Monitor.Trace().Send( "TransactionnalEventPublisher correctly enlisted in the current transaction." );
                    }
                    var evt =  CreateEvent( @event );
                    _unCommitedEvents.Enqueue( evt );
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
            if( _delegate == null )
                throw new InvalidOperationException( "An event emitter must be configured by calling CommandExecutionContext.SetEventEmitter" );

            _delegate( evt );
        }

        public void Commit( Enlistment enlistment )
        {
            _context.Monitor.Trace().Send( "Transaction is commit, we clear sent events...." );

            _unCommitedEvents.Clear();

            enlistment.Done();
        }

        public void InDoubt( Enlistment enlistment )
        {
            _context.Monitor.Trace().Send( "InDoubt transaction..." );
            enlistment.Done();
        }

        //Don't throw an exception here. Instead call ForceRollback()
        public void Prepare( PreparingEnlistment preparingEnlistment )
        {
            using( _context.Monitor.OpenTrace().Send( "Preparing event publishing..." ) )
            {
                try
                {
                    while( _unCommitedEvents.Count > 0 )
                    {
                        var evt = _unCommitedEvents.Dequeue();
                        PublishEvent( evt );
                        _context.Monitor.Trace().Send( "Event {0} published.", evt.EventType );
                    }
                    preparingEnlistment.Prepared();
                }
                catch( Exception e )
                {
                    _context.Monitor.Error().Send( e );
                    preparingEnlistment.ForceRollback( e );
                }
            }
        }
        public void Rollback( Enlistment enlistment )
        {
            _context.Monitor.Trace().Send( "Rollbacking transaction and clearing all uncommited events..." );

            _unCommitedEvents.Clear();

            enlistment.Done();
        }

        public static void SetEventEmitter( Func<Event, Task> emitter )
        {
            _delegate = emitter;
        }
    }
}
