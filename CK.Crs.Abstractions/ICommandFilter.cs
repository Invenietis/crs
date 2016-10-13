using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// <see cref="ICommandFilter"/> is invoked during command receiving.
    /// 
    /// </summary>
    public interface ICommandFilter
    {
        int Order { get; }

        /// <summary>
        /// Used to filter the command. To a command, you should call context.Reject() and specify the reason.
        /// </summary>
        /// <param name="context">The <see cref="ICommandFilterContext"/>.</param>
        /// <returns></returns>
        Task OnCommandReceived( ICommandFilterContext context );
    }

}