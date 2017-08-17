//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace CK.Crs
//{
//    public abstract class CommandHandlerBase<T, TResult> : ICommandHandlerWithResult<T>
//    {
//        public abstract Task<TResult> HandleAsync( T command, ICommandContext context );

//        Task<object> ICommandHandlerWithResult<T>.HandleAsync( T command, ICommandContext context )
//        {
//            return (Task<object>)(object)HandleAsync( command, context );
//        }
//    }
//}
