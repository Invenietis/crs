using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandFiltersInvoker : PipelineSlotBase
    {
        readonly IFactories _factories;

        public CommandFiltersInvoker( CommandReceivingPipeline pipeline, IFactories factories ) : base( pipeline )
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

        public override async Task Invoke( CancellationToken token )
        {
            if( ShouldInvoke )
            {
                var filterContext = new FilterContext(Pipeline.Request.Monitor, Pipeline.Request.CommandDescription, Pipeline.Request.User, Pipeline.Request.Command);

                using( Pipeline.Request.Monitor.OpenTrace().Send( "Applying filters..." )
                    .ConcludeWith( () => filterContext.IsRejected ? "INVALID" : "OK" ) )
                {
                    foreach( var filter in Pipeline.Request.CommandDescription.Filters.Select( f => new FilterInfo( _factories.CreateFilter( f ) ) ) )
                    {
                        if( filter.Instance == null )
                        {
                            string msg = $"Unable to create the filter {filter.Type.FullName}.";
                            throw new InvalidOperationException( msg );
                        }
                        using( Pipeline.Request.Monitor.OpenTrace().Send( $"Executing filter {filter.Type.Name}" ) )
                        {
                            await filter.Instance.OnCommandReceived( filterContext );

                            // Immediatly test if there is a response available.
                            if( filterContext.IsRejected )
                            {
                                Pipeline.Response = new CommandInvalidResponse( Pipeline.CommandId, filterContext.RejectReason );
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
