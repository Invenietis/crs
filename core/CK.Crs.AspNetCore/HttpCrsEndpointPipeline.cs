using CK.Core;
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
        static readonly IEnumerable<ICrsFilter> _filters = new ICrsFilter[] { new AmbientValuesValidationFilter(), new ModelValidationFilter() };

        public bool IsValid => Context != null;

        public IEndpointModel Model { get; }

        public ICommandContext Context { get; }

        public HttpContext HttpContext { get; }

        public IActivityMonitor Monitor { get; }

        public HttpCrsEndpointPipeline( IActivityMonitor monitor, IEndpointModel model, ICommandContext context, HttpContext httpContext )
        {
            Monitor = monitor ?? throw new ArgumentNullException( nameof( monitor ) );
            Model = model ?? throw new ArgumentNullException( nameof( model ) );
            Context = context;
            HttpContext = httpContext;
        }

        public virtual async Task<Response> ProcessCommand()
        {
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
            var binder = ActivatorUtilities.CreateInstance(
                HttpContext.RequestServices,
                Context.Model.Binder ?? Model.Binder,
                HttpContext ) as ICommandBinder;

            return binder.Bind( Context );
        }

        public virtual IEnumerable<ICrsFilter> GetFilters() => _filters;

        public async Task<Response> ApplyFilters( object command )
        {
            if( command == null )
            {
                throw new ArgumentNullException( nameof( command ) );
            }
            CrsFilterContext context = new CrsFilterContext( command, Context, Model, HttpContext );
            foreach( var filter in GetFilters() )
            {
                await filter.ApplyFilterAsync( context );
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
                if( Model.ApplyAmbientValuesValidation )
                {
                    IAmbientValues ambientValues = HttpContext.RequestServices.GetRequiredService<IAmbientValues>();
                    IAmbientValuesRegistration ambientValueRegistration = HttpContext.RequestServices.GetRequiredService<IAmbientValuesRegistration>();

                    return await MetaCommand.Result.CreateAsync( metaCommand, Model, ambientValues, ambientValueRegistration );
                }
                else
                {
                    return MetaCommand.Result.Create( metaCommand, Model );
                }
            }
            return null;
        }

        public void Dispose()
        {
        }
    }
}
