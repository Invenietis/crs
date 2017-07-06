using CK.Core;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICrsEndpoint<T> where T : class
    {
        Task<CommandResponse> ReceiveCommand(T command, IActivityMonitor monitor, string callerId);
    }
}
