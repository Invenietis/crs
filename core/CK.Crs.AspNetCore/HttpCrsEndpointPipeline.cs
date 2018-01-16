using CK.Crs;
using CK.Crs.Meta;
using CK.Crs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore
{
    class HttpCrsEndpointPipeline : ICrsEndpointPipeline
    {
        public bool IsValid => Context != null;

        public IEndpointModel Model { get; }

        public ICommandContext Context { get; }

        public HttpContext HttpContext { get; }

        public CK.Core.IActivityMonitor Monitor { get; }

        public HttpCrsEndpointPipeline( CK.Core.IActivityMonitor monitor, IEndpointModel model, ICommandContext context, HttpContext httpContext )
        {
            Monitor = monitor ?? throw new ArgumentNullException( nameof( monitor ) );
            Model = model ?? throw new ArgumentNullException( nameof( model ) );
            Context = context;
            HttpContext = httpContext;
        }

        public virtual async Task<Response> ProcessCommand()
        {
            if( !IsValid ) throw new InvalidOperationException( "Invalid pipeline state!" );

            object command = await BindCommand();
            if( command == null ) throw new InvalidOperationException( "Unable to bind the input command" );

            Response response = await GetMeta( command );
            if( response != null ) return response;

            response = await ApplyFilters( command );
            if( response != null ) return response;

            ICommandReceiver commandReceiver = HttpContext.RequestServices.GetRequiredService<ICommandReceiver>();
            response = await commandReceiver.ReceiveCommand( command, Context );
            if( response != null ) return response;

            return null;
        }

        public virtual Task<object> BindCommand()
        {
            var binder = HttpContext.RequestServices.GetRequiredService( Context.Model.Binder ?? Model.Binder ) as ICommandBinder;
            return binder.Bind( Context );
        }

        public virtual IEnumerable<ICommandFilter> GetFilters()
            => HttpContext.RequestServices.GetRequiredService<ICommandFilterProvider>().GetFilters( Context, Model );

        public async Task<Response> ApplyFilters( object command )
        {
            if( command == null )
            {
                throw new ArgumentNullException( nameof( command ) );
            }

            CommandFilterContext context = new CommandFilterContext( command, Context, Model );
            foreach( var filter in GetFilters() )
            {
                await filter.OnFilterAsync( context );
                if( context.Response != null )
                {
                    return context.Response;
                }
            }
            return null;
        }


        static readonly InvalidResponse InvalidMetaCommand = new InvalidResponse( Guid.Empty.ToString( "N" ), "Meta command malformed" );
        public async Task<Response> GetMeta( object command )
        {
            if( command is MetaCommand metaCommand )
            {
                var ambientValues = HttpContext.RequestServices.GetService<CK.Core.IAmbientValues>();
                var ambientValueRegistration = HttpContext.RequestServices.GetService<CK.Core.IAmbientValuesRegistration>();

                return await MetaCommand.Result.CreateAsync( metaCommand, Model, ambientValues, ambientValueRegistration );

            }
            return null;
        }

        public void Dispose()
        {
            if( Context != null )
            {
                Context.SetFeature<IHttpContextCommandFeature>( null );
                Context.SetFeature<IRequestServicesCommandFeature>( null );
            }
        }
    }
}
