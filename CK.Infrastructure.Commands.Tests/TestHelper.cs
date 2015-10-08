using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace CK.Infrastructure.Commands.Tests
{
    public class TestHelper
    {
        public static IServiceProvider CreateServiceProvider(  Action<IServiceCollection> setup )
        {
            var services = new ServiceCollection();
            if( setup != null )
            {
                setup( services );
            }
            
            return services.BuildServiceProvider();
        }
    }
}
