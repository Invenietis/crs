﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{

    /// <summary>
    /// Raises events during the command processing at any stage of the pipeline.
    /// </summary>
    public class PipelineEvents
    {
        /// <summary>
        /// Called when a command has been concretised into a <see cref="CommandAction"/>
        /// </summary>
        public Func<CommandAction, Task> CommandBuilt { get; set; }

        public Func<AmbientValueValidationContext, Task> AmbientValuesValidating { get; set; }

        public Func<AmbientValueValidationContext, Task> AmbientValuesValidated { get; set; }

        /// <summary>
        /// Called when the command has been rejected
        /// </summary>
        public Func<CommandRejectedContext, Task> CommandRejected { get; set; }

        /// <summary>
        /// Called when the command is being executing during execution phasis.
        /// </summary>
        public Func<CommandContext, Task> CommandExecuting { get; set; }
    }
}
