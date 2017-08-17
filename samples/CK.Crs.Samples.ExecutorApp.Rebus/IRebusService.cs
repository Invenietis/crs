using Microsoft.Extensions.DependencyInjection;
using System;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    public interface IRebusService
    {
        void Init( IServiceCollection services );
        void Start( IServiceProvider services );
        void Stop( IServiceProvider services );
    }
}
