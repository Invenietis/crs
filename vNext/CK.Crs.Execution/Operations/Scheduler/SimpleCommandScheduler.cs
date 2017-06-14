﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Execution
{
    public class SimpleCommandScheduler : ICommandScheduler
    {
        readonly IOperationExecutor<ScheduledCommand> _operationExecutor;
        readonly ICommandRegistry _commandRegistry;

        public SimpleCommandScheduler( ICommandRegistry commandRegistry, IOperationExecutor<ScheduledCommand> operationExecutor )
        {
            _commandRegistry = commandRegistry ?? throw new ArgumentNullException( nameof( commandRegistry ) );
            _operationExecutor = operationExecutor ?? throw new ArgumentNullException( nameof( operationExecutor ) );
        }

        public bool CancelScheduling( IActivityMonitor mointor, Guid commandId )
        {
            mointor.Warn().Send( "Cancel a scheduled command is not supported yet." );
            return false;
        }

        public Guid Schedule<T>( IActivityMonitor monitor, T command, CommandSchedulingOption option )
        {
            var description = _commandRegistry.Registration.FirstOrDefault( x => x.CommandType == typeof(T));
            if( description == null )
            {
                throw new InvalidOperationException( $"Unable to schedule command of type {typeof( T ).Name}. Make sure it is registered on the global ICommandRegistry." );
            }

            ScheduledCommand operation = new ScheduledCommand(monitor, CreateCommandIdentifier() )
            {
                Command = command,
                Description = description,
                Scheduling = option
            };

            _operationExecutor.Execute( monitor, operation );

            //if( Transaction.Current == null )
            //{
            //    _operationExecutor.Execute( monitor, operation );
            //}
            //else
            //{
            //    TransactionOperationManager<ScheduledCommand>.AddOperation( monitor, _operationExecutor, operation );
            //}

            return operation.CommandId;
        }

        protected virtual Guid CreateCommandIdentifier()
        {
            return Guid.NewGuid();
        }
    }
}