using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Pipeline
{
    public class PipelineBuilder : IPipelineBuilder
    {
        private readonly LinkedList<Func<IPipeline, Task<IPipeline>>> _list;
        private readonly IPipelineComponentFactory _factory;

        public PipelineBuilder( IPipelineComponentFactory factory )
        {
            _factory = factory;
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

        public T CreateComponent<T>( IPipeline pipeline ) where T : PipelineComponent
        {
            return _factory.CreateComponent( pipeline, typeof( T ) ) as T;
        }

        public IPipelineBuilder Clear()
        {
            _list.Clear();
            return this;
        }
    }
}
