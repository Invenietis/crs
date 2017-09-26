using System;
namespace CK.Crs
{
    public interface IResultDispatcherSelector
    {
        /// <summary>
        /// Selects the appropriate dispatcher regarding the <see cref="ICommandContext"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IResultDispatcher SelectDispatcher( ICommandContext context );
    }
}
