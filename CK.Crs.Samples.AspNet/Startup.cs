using CK.Core;
using CK.Crs.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web;

[assembly: OwinStartup( typeof( CK.Crs.Samples.AspNet.Startup ) )]

namespace CK.Crs.Samples.AspNet
{
    public interface IHttpContextAccessor
    {
        HttpContextBase HttpContext { get; }

        HttpContextBase EnsureHttpContext();
    }

    public class HttpContextAccessor : IHttpContextAccessor
    {
        public HttpContextBase HttpContext { get; private set; }

        public HttpContextBase EnsureHttpContext()
        {
            if( HttpContext == null ) HttpContext = new HttpContextWrapper( System.Web.HttpContext.Current );
            return HttpContext;
        }
    }

    public class Startup
    {
        public void Configuration( IAppBuilder app )
        {
            WeakAssemblyNameResolver.Install();

            var applicationServices = ConfigureServices( new ServiceCollection() );
            ConfigureApplication( app, applicationServices );
        }


        public IServiceProvider ConfigureServices( ServiceCollection services )
        {
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAmbientValues( r =>
            {
                r.AddProvider<ActorAmbientValueProvider>( "ActorId" );
            } );
            services.AddCrsCore<ScopedBasedCommandHandlerActivator>( r =>
            {
                r.Register<SampleCommand, SampleHandler>().CommandName( "Sample" );
            } );
            return services.BuildServiceProvider();
        }
        public void ConfigureApplication( IAppBuilder app, IServiceProvider applicationServices )
        {
            var serviceScopeFactory = applicationServices.GetRequiredService<IServiceScopeFactory>();
            app.Use( async ( context, next ) =>
            {
                var scope = serviceScopeFactory.CreateScope();
                context.Set( nameof( IServiceScope ), scope );

                var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor.EnsureHttpContext();
                httpContext.Items.Add( typeof( IServiceScope ), scope );
                //context.Request.Set( typeof( IServiceScope ).Name, scope );

                try
                {
                    await next().ConfigureAwait( false );
                }
                finally
                {
                    scope.Dispose();
                    httpContext.Items.Remove( typeof( IServiceScope ) );
                }
            } );
            app.UseCrsOwin( "/commands", applicationServices, configuration =>
            {
                configuration.AcceptAll();
                //configuration.AddSecurityFilter<AuthenticatedFilter>();
            } );

            //app.Use( ( ctx, next ) =>
            //{
            //    ctx.Response.Write( "Lol" );
            //    //return next();
            //    return Task.
            //} );
        }
    }
    public class ActorAmbientValueProvider : IAmbientValueProvider
    {
        private readonly IHttpContextAccessor _httpContext;

        public Type ValueType => typeof( int );

        public ActorAmbientValueProvider( IHttpContextAccessor httpContextAccessor )
        {
            _httpContext = httpContextAccessor;
        }

        public Task<IComparable> GetValueAsync( IAmbientValues values )
        {
            return Task.FromResult<IComparable>( _httpContext.HttpContext.User.Identity.Name.GetHashCode() );
        }
    }
    class AuthenticatedFilter : ICommandSecurityFilter
    {
        private readonly IHttpContextAccessor _httpContext;
        public AuthenticatedFilter( IHttpContextAccessor httpContextAccessor )
        {
            _httpContext = httpContextAccessor;
        }

        public Task OnFilterAsync( CommandFilterContext filterContext )
        {
            if( _httpContext.HttpContext == null || !_httpContext.HttpContext.User.Identity.IsAuthenticated )
            {
                filterContext.SetResponse( new InvalidResponse( filterContext.CommandContext.CommandId, "Unauthorized" ) );
            }
            return Task.CompletedTask;
        }
    }
    public class SampleCommand
    {
        public int ActorId { get; set; }
        public float[] HeatingPower { get; set; }
    }

    public class SampleHandler : ICommandHandler<SampleCommand>
    {
        public Task HandleAsync( SampleCommand command, ICommandContext context )
        {
            return Task.CompletedTask;
        }
    }

    public class ScopedBasedCommandHandlerActivator : ICommandHandlerActivator
    {
        private readonly IHttpContextAccessor _httpContext;

        public ScopedBasedCommandHandlerActivator( IHttpContextAccessor httpContext )
        {
            _httpContext = httpContext;
        }
        public object Create( Type serviceType )
        {
            var provider = _httpContext.HttpContext.GetRequestServices();
            try
            {
                return provider.GetService( serviceType ) ?? ActivatorUtilities.CreateInstance( provider, serviceType );
            }
            catch
            {
                return null;
            }
        }

        public void Release( object o )
        {
            if( o is IDisposable d ) d.Dispose();
        }
    }

    public static class HttpServiceScopeExtensions
    {
        public static IServiceProvider GetRequestServices( this HttpContextBase httpContext )
        {
            if( httpContext == null )
            {
                throw new ArgumentNullException( nameof( httpContext ) );
            }

            if( httpContext.Items[typeof( IServiceScope )] is IServiceScope scope )
            {
                return scope.ServiceProvider;
            }

            throw new InvalidOperationException( "There must be a ServiceScope registered in the Items of the HttpContext." );
        }
    }
}
