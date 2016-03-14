using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class PipelineEvents
    {
        public Func<ICommandFilterContext, Task> CommandRejectedByFilter { get; set; }

        public Func<ICommandExecutionContext, Task> CommandExecuting { get; set; }

        public Func<ICommandExecutionContext, Task> CommandExecuted { get; set; }

        public Func<CommandAction, Task> CommandBuilt { get; set; }

        public Func<AmbientValueValidationContext, Task> ValidatingAmbientValues { get; set; }

        public Func<AmbientValueValidationContext, Task> AmbientValuesValidated { get; set; }
    }
}
