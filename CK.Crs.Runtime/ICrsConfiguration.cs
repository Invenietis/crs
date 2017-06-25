﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    /// <summary>
    /// Holds configuration specific to a CRS Handler.
    /// </summary>
    public interface ICrsConfiguration : ITraitContextProvider
    {
        /// <summary>
        /// Gets the receiver path.
        /// </summary>
        string ReceiverPath { get; }

        /// <summary>
        /// Gets the <see cref="PipelineEvents"/> 
        /// </summary>
        PipelineEvents Events { get; }

        /// <summary>
        /// Gets the <see cref="IPipelineBuilder"/>
        /// </summary>
        IPipelineBuilder Pipeline { get; }

        /// <summary>
        /// External components configuration
        /// </summary>
        ExternalComponents ExternalComponents { get; }
    }

}
