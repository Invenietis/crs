using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    /// <summary>
    /// Basic implementation of the <see cref="IPipelineBuilder"/>.
    /// </summary>
    public class PipelineBuilder : IPipelineBuilder
    {
        private readonly LinkedList<Func<IPipeline, Task<IPipeline>>> _list;

        public PipelineBuilder()
        {
            _list = new LinkedList<Func<IPipeline, Task<IPipeline>>>();
        }

        public IEnumerable<Func<IPipeline, Task<IPipeline>>> Components
        {
            get { return _list; }
        }

        public IPipelineBuilder Use( Func<IPipeline, Task<IPipeline>> inlineComponent )
        {
            _list.AddLast( inlineComponent );
            return this;
        }

        public IPipelineBuilder Use<T>() where T : PipelineComponent
        {
            return Use( pipeline =>
            {
                using( pipeline.Monitor.OpenTrace().Send( "Invoking component {0}", typeof( T ).Name ) )
                {
                    var component = ActivatorUtilities.GetServiceOrCreateInstance<T>( pipeline.CommandServices );
                    if( component == null )
                        throw new InvalidOperationException( String.Format( "The component {0} has not been created.", typeof( T ).Name ) );

                    return component.TryInvoke( pipeline, pipeline.CancellationToken );
                }
            } );
        }

        public IPipelineBuilder Clear()
        {
            _list.Clear();
            return this;
        }
    }
}
