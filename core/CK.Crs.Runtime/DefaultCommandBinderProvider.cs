using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Runtime
{
    internal class DefaultCommandBinderProvider : ICommandBinderProvider
    {
        public DefaultCommandBinderProvider( IServiceProvider services )
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        public ICommandBinder CreateBinder( ICommandContext context, IEndpointModel model )
        {
            return ActivatorUtilities.CreateInstance( Services, context.Model.Binder ?? model.Binder ) as ICommandBinder;
        }
    }
}
