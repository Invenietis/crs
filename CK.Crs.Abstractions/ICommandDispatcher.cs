using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandDispatcher
    {
        Task<object> SendAsync<T>(T command, ICommandContext context) where T : class;

        Task PublishAsync<T>( T evt, ICommandContext context) where T : class;
    }
}
