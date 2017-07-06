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
        Task SendAsync<T>(T command, ICommandExecutionContext context) where T : class;
    }
}
