using CK.Crs.Runtime;
using Paramore.Brighter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Samples.AspNetCoreApp.Core
{
    public interface ICrsEndpoint<T> where T : class, ICommand
    {
        Task<CommandResponse> ReceiveCommand(T command, string callbackId);
    }
}
