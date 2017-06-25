using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    public interface ICommandRunningStore
    {
        Task AddCommandAsync( string callbackId, Guid commandId );

        Task RemoveCommandAsync( string callbackId, Guid commandId );

        Task<IReadOnlyCollection<Guid>> GetRunningCommands( string callbackId );
    }
}
