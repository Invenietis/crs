using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Publishs an event on a local or distributed channel depending of implementations.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishs the event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="evt"></param>
        /// <param name="context">The related <see cref="ICommandContext"/> of the original command.</param>
        /// <returns></returns>
        Task PublishAsync<T>( T evt, ICommandContext context) where T : class;
    }
}
