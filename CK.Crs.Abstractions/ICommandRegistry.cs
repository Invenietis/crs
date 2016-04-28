﻿using System.Collections.Generic;

namespace CK.Crs
{
    /// <summary>
    /// Defines a command registry, which commands are globaly stored.
    /// </summary>
    public interface ICommandRegistry
    {
        void Register( CommandDescription descriptor );

        IEnumerable<CommandDescription> Registration { get; }
    }
}