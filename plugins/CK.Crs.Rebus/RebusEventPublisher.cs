//using System.Collections.Generic;
//using System.Threading.Tasks;
//using CK.Core;
//using R = Rebus;

//namespace CK.Crs
//{
//    public class RebusEventPublisher : IEventPublisher
//    {
//        readonly R.Bus.IBus _bus;

//        public RebusEventPublisher( R.Bus.IBus bus )
//        {
//            _bus = bus;
//        }

//        public Task PublishAsync<T>( T evt, ICommandContext context ) where T : class
//        {
//            var h = ToHeaders( context );
//            return _bus.Publish( evt, h );
//        }

//        private Dictionary<string, string> ToHeaders( ICommandContext context )
//        {
//            return new Dictionary<string, string>
//            {
//                // CRS Headers
//                { nameof( ICommandContext.CallerId ), context.CallerId },
//                { nameof( ICommandContext.CommandId ), context.CommandId.ToString() },
//                { nameof( ICommandContext.Monitor ), context.Monitor.DependentActivity().CreateToken().ToString() },
//                // Rebus Headers
//                { R.Messages.Headers.MessageId, context.CommandId.ToString() }
//            };
//        }
//    }
//}
