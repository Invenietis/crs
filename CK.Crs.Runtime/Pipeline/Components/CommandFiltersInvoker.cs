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
        readonly ICommandFilterFactory _commandFilterFactory;

        public CommandFiltersInvoker( ICommandFilterFactory commandFilterFactory )
        {
            _commandFilterFactory = commandFilterFactory;
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

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null && pipeline.Action.Command != null;
        }

        public override async Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            var filterContext = new FilterContext(pipeline.Monitor, pipeline.Action.Description, pipeline.Request.User, pipeline.Action.Command);

            using( pipeline.Monitor.OpenTrace().Send( "Applying filters..." )
                .ConcludeWith( () => filterContext.Rejected ? "INVALID" : "OK" ) )
            {
                foreach( var filter in pipeline.Action.Description.Filters.Select( f => new FilterInfo( _commandFilterFactory.CreateFilter( f ) ) ) )
                {
                    if( filter.Instance == null )
                    {
                        string msg = $"Unable to create the filter {filter.Type.FullName}.";
                        throw new InvalidOperationException( msg );
                    }
                    using( pipeline.Monitor.OpenTrace().Send( $"Executing filter {filter.Type.Name}" ) )
                    {
                        await filter.Instance.OnCommandReceived( filterContext );

                        // Immediatly test if there is a response available.
                        if( filterContext.Rejected )
                        {
                            var rejectedContext = new CancellableCommandRejectedContext( pipeline.Action, filterContext);
                            await pipeline.Configuration.Events.CommandRejected?.Invoke( rejectedContext );

                            if( rejectedContext.Canceled == false )
                            {
                                pipeline.Response = new CommandInvalidResponse( pipeline.Action.CommandId, filterContext.RejectReason );
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
