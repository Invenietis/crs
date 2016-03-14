#if NET451
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class TransactionnalCommandScheduler : ICommandScheduler
    {
        static Func<IOperationExecutor<ScheduledCommand>> _operationExecutorThunk;
        public static void SetEventEmitter( Func<IOperationExecutor<ScheduledCommand>> operationExecutor )
        {
            _operationExecutorThunk = operationExecutor;
        }

        readonly ICommandExecutionContext _context;
        readonly IOperationExecutor<ScheduledCommand> _operationExecutor;
        readonly Lazy<TransactionOperationManager<ScheduledCommand>> _resourceManager;

        public TransactionnalCommandScheduler( ICommandExecutionContext context ) : this( context, _operationExecutorThunk() )
        {
        }

        public TransactionnalCommandScheduler( ICommandExecutionContext context, IOperationExecutor<ScheduledCommand> operationExecutor )
        {
            if( context == null ) throw new ArgumentNullException( nameof( context ) );
            if( operationExecutor == null ) throw new ArgumentNullException( nameof( operationExecutor ) );

            _context = context;
            _operationExecutor = operationExecutor;
            _resourceManager = new Lazy<TransactionOperationManager<ScheduledCommand>>(
                () => new TransactionOperationManager<ScheduledCommand>( context.Monitor, operationExecutor ) );
        }

        public bool CancelScheduling( Guid commandId )
        {
            _context.Monitor.Warn().Send( "Cancel a scheduled command is not supported yet." );
            return false;
        }

        public Guid Schedule<T>( T command, CommandSchedulingOption option )
        {
            ScheduledCommand operation = new ScheduledCommand( CreateCommandIdentifier(), option.ClaimsPrincipal )
            {
                Command = command,
                Description = null, // TODO: obtain description for the command to schedule.
                Scheduling = option
            };

            if( Transaction.Current == null )
            {
                _operationExecutor.Execute( _context.Monitor, operation );
            }
            else
            {
                _resourceManager.Value.AddOperation( operation );
            }

            return operation.CommandId;
        }

        protected virtual Guid CreateCommandIdentifier()
        {
            return Guid.NewGuid();
        }
    }
}
#endif