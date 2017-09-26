namespace CK.Crs
{
    public interface IResultReceiverProvider
    {
        /// <summary>
        /// Gets the appropriate <see cref="IResultReceiver"/>
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        IResultReceiver GetResultReceiver( ICommandContext context );
    }
}
