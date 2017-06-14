//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Transactions;
//using CK.Core;

//namespace CK.Crs.Runtime.Execution
//{
//    internal class TransactionOperationManager<T> : IEnlistmentNotification
//    {
//        readonly IOperationExecutor<T> _operationExecutor;
//        readonly IActivityMonitor _monitor;
//        readonly Queue<T> _queuedOperations;

//        TransactionOperationManager( IActivityMonitor monitor, IOperationExecutor<T> operationExecutor )
//        {
//            _monitor = monitor;
//            _operationExecutor = operationExecutor;
//            _queuedOperations = new Queue<T>();

//            Transaction.Current.EnlistVolatile( this, EnlistmentOptions.None );
//        }

//        static public void AddOperation( IActivityMonitor monitor, IOperationExecutor<T> executor, T operation )
//        {
//            var m =new TransactionOperationManager<T>( monitor, executor );
//            m._queuedOperations.Enqueue( operation );
//        }

//        void IEnlistmentNotification.Commit( Enlistment enlistment )
//        {
//            _monitor.Trace().Send("Transaction is commit, we clear sent events...." );

//            _queuedOperations.Clear();

//            enlistment.Done();
//        }

//        void IEnlistmentNotification.InDoubt( Enlistment enlistment )
//        {
//            _monitor.Trace().Send("InDoubt transaction..." );
//            enlistment.Done();
//        }

//        //Don't throw an exception here. Instead call ForceRollback()
//        void IEnlistmentNotification.Prepare( PreparingEnlistment preparingEnlistment )
//        {
//            using( _monitor.OpenTrace().Send("Preparing operations execution..." ) )
//            {
//                try
//                {
//                    while( _queuedOperations.Count > 0 )
//                    {
//                        var operation = _queuedOperations.Dequeue();
//                        _operationExecutor.Execute( _monitor, operation );
//                        _monitor.Trace().Send("Operation executed." );
//                    }
//                    preparingEnlistment.Prepared();
//                }
//                catch( Exception e )
//                {
//                    _monitor.Error().Send( e );
//                    preparingEnlistment.ForceRollback( e );
//                }
//            }
//        }
//        void IEnlistmentNotification.Rollback( Enlistment enlistment )
//        {
//            _monitor.Trace().Send("Rollbacking transaction and clearing all uncommited events..." );

//            _queuedOperations.Clear();

//            enlistment.Done();
//        }
//    }
//}
