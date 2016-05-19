using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using CK.Core;

namespace CK.Crs.Runtime.Execution
{
    class TransactionnalCommandScheduler : ICommandScheduler
    {
        readonly IOperationExecutor<ScheduledCommand> _operationExecutor;
        readonly IPipeline _originalPipeline;

        public TransactionnalCommandScheduler( IOperationExecutor<ScheduledCommand> operationExecutor, IPipeline pipeline )
        {
            if( operationExecutor == null ) throw new ArgumentNullException( nameof( operationExecutor ) );

            _operationExecutor = operationExecutor;
            _originalPipeline = pipeline;
        }

        public bool CancelScheduling( IActivityMonitor mointor, Guid commandId )
        {
            mointor.Warn().Send( "Cancel a scheduled command is not supported yet." );
            return false;
        }

        public Guid Schedule<T>( IActivityMonitor monitor, T command, CommandSchedulingOption option )
        {
            ScheduledCommand operation = new ScheduledCommand( CreateCommandIdentifier() )
            {
                Command = command,
                Description = null, // TODO: obtain description for the command to schedule.
                Scheduling = option,
                Pipeline = _originalPipeline
            };

            if( Transaction.Current == null )
            {
                _operationExecutor.Execute( monitor, operation );
            }
            else
            {
                TransactionOperationManager<ScheduledCommand>.AddOperation( monitor, _operationExecutor, operation );
            }

            return operation.CommandId;
        }

        protected virtual Guid CreateCommandIdentifier()
        {
            return Guid.NewGuid();
        }
    }
}
