using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Filtering
{
    public class DefaultCommandFilterFactory : ICommandFilterFactory
    {
        IServiceProvider _serviceProvider;
        public DefaultCommandFilterFactory( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }

        public ICommandFilter CreateFilter( Type filterType ) => _serviceProvider.GetService( filterType ) as ICommandFilter;
    }
}
