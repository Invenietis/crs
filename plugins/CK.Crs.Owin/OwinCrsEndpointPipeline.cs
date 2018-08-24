using CK.Crs.Meta;
using CK.Crs.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Crs.Owin
{
    class OwinCrsEndpointPipeline : ICrsEndpointPipeline
    {
        public bool IsValid => Context != null;

        public IEndpointModel Model { get; }

        public ICommandContext Context { get; }

        public IOwinContext HttpContext { get; }

        public IServiceProvider ApplicationServices { get; }

        public CK.Core.IActivityMonitor Monitor { get; }

        public OwinCrsEndpointPipeline( CK.Core.IActivityMonitor monitor, IEndpointModel model, ICommandContext context, IOwinContext owinContext, IServiceProvider applicationServices )
        {
            Monitor = monitor ?? throw new ArgumentNullException( nameof( monitor ) );
            Model = model ?? throw new ArgumentNullException( nameof( model ) );
            Context = context;
            HttpContext = owinContext;
            ApplicationServices = applicationServices;
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

            ICommandReceiver commandReceiver = ApplicationServices.GetRequiredService<ICommandReceiver>();
            response = await commandReceiver.ReceiveCommand( command, Context );
            if( response != null ) return response;

            return null;
        }

        public virtual Task<object> BindCommand()
        {
            var binder = ApplicationServices.GetRequiredService( Context.Model.Binder ?? Model.Binder ) as ICommandBinder;
            return binder.Bind( Context );
        }

        public virtual IEnumerable<ICommandFilter> GetFilters()
            => ApplicationServices.GetRequiredService<ICommandFilterProvider>().GetFilters( Context, Model );

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
                return await MetaCommand.Result.CreateAsync( metaCommand, Model, ApplicationServices );
            }
            return null;
        }
    }
}
