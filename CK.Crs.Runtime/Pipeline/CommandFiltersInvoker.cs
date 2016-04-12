using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandFiltersInvoker : PipelineComponent
    {
        readonly IFactories _factories;

        public CommandFiltersInvoker( IPipeline pipeline, IFactories factories ) : base( pipeline )
        {
            _factories = factories;
        }

        class FilterInfo
        {
            public FilterInfo( ICommandFilter instance )
            {
                instance = Instance;
                Type = instance.GetType();
            }

            public Type Type { get; set; }
            public ICommandFilter Instance { get; set; }
        }

        public override bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Action.Command != null; }
        }

        public override async Task Invoke( CancellationToken token )
        {
            var filterContext = new FilterContext(Pipeline.Monitor, Pipeline.Action.Description, Pipeline.Request.User, Pipeline.Action.Command);

            using( Pipeline.Monitor.OpenTrace().Send( "Applying filters..." )
                .ConcludeWith( () => filterContext.Rejected ? "INVALID" : "OK" ) )
            {
                foreach( var filter in Pipeline.Action.Description.Filters.Select( f => new FilterInfo( _factories.CreateFilter( f ) ) ) )
                {
                    if( filter.Instance == null )
                    {
                        string msg = $"Unable to create the filter {filter.Type.FullName}.";
                        throw new InvalidOperationException( msg );
                    }
                    using( Pipeline.Monitor.OpenTrace().Send( $"Executing filter {filter.Type.Name}" ) )
                    {
                        await filter.Instance.OnCommandReceived( filterContext );

                        // Immediatly test if there is a response available.
                        if( filterContext.Rejected )
                        {
                            var rejectedContext = new CancellableCommandRejectedContext( Pipeline.Action, filterContext);
                            await Pipeline.Events.CommandRejected?.Invoke( rejectedContext );

                            if( rejectedContext.Canceled == false )
                            {
                                Pipeline.Response = new CommandInvalidResponse( Pipeline.Action.CommandId, filterContext.RejectReason );
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
