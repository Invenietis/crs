﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    class AsyncExecutionStrategy : IExecutionStrategy
    {
        readonly CancellationTokenSource _cancellationSource;
        readonly Func<ICommandResponseDispatcher> _commandResponseDispatcher;
        readonly CommandRunner _runner;

        public AsyncExecutionStrategy( CommandRunner runner, Func<ICommandResponseDispatcher> dispatcher )
        {
            if( runner == null ) throw new ArgumentNullException( nameof( runner ) );
            if( dispatcher == null ) throw new ArgumentNullException( nameof( dispatcher ) );

            _commandResponseDispatcher = dispatcher;
            _runner = runner;
            _cancellationSource = new CancellationTokenSource();
        }

        public Task<CommandResponse> ExecuteAsync( CommandContext context )
        {
            var token = context.ExecutionContext.Monitor.DependentActivity().CreateTokenWithTopic( GetType().Name );

            // This implementation does not guarantee that the command will be correctly handled...
            // We need some retry mechanism and a pending command persistence mechanism to be resilient.
            var t = new Task( async () =>
            {
                // We override the IActivityMonitor with a dependant one to be thread safe !
                using( var dependentMonitor = token.CreateDependentMonitor() )
                {
                    context.ExecutionContext.Monitor = dependentMonitor;
                    context.ExecutionContext.CommandAborted = _cancellationSource.Token;

                    await _runner.ExecuteAsync( context );

                    var response = new CommandResultResponse( context.Result, context.ExecutionContext );
                    var responseDispatcher = _commandResponseDispatcher();
                    if( responseDispatcher == null ) 
                        dependentMonitor.Warn().Send("No response dispatcher were available...");
                    else
                        await responseDispatcher.DispatchAsync( context.ExecutionContext.CallbackId, response );
                }
            } );

            var deferredResponse = new CommandDeferredResponse( context.ExecutionContext );
            t.Start( TaskScheduler.Current );
            return Task.FromResult<CommandResponse>( deferredResponse );
        }
    }
}
