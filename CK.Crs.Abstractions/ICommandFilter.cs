﻿using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface ICommandFilter : IInjectable
    {
        int Order { get; }
        Task OnCommandReceived( CommandContext context );
    }
}