using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Crs.Scalability.Internals
{
    class DirectCommandRouterComponent : PipelineComponent
    {
        readonly ICommandRegistry _registry;
        public DirectCommandRouterComponent( ICommandRegistry registry )
        {
            _registry = registry;
        }

        public override Task Invoke(IPipeline pipeline, CancellationToken token = default(CancellationToken))
        {
            var commandDescription = _registry.Registration.SingleOrDefault(c => c.CommandType.Name == pipeline.Request.Path.CommandName);
            if (commandDescription != null)
            {
                pipeline.Action.CallbackId = pipeline.Request.CallbackIdentifier;
                pipeline.Action.Description = commandDescription;
            }
            else
            {
                pipeline.Response.Set(new CommandInvalidResponse(pipeline.Action.CommandId, "Command does not exists"));
            }

            return Task.CompletedTask;
        }

        public override bool ShouldInvoke(IPipeline pipeline)
        {
            return !pipeline.Response.HasReponse;
        }
    }
}
