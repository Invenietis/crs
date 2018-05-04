using CK.Core;
using CK.Crs.Meta;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CrsPipeline : ICrsEndpointPipeline
    {
        public bool IsValid => Context != null;

        public IEndpointModel Model { get; }

        public ICommandContext Context { get; }

        public IActivityMonitor Monitor { get; }

        protected IServiceProvider Services { get; }

        public CrsPipeline( IActivityMonitor monitor, IEndpointModel model, IServiceProvider services, ICommandContext context )
        {
            Monitor = monitor ?? throw new ArgumentNullException( nameof( monitor ) );
            Model = model ?? throw new ArgumentNullException( nameof( model ) );
            Services = services ?? throw new ArgumentNullException( nameof( services ) );
            Context = context;
        }

        public async Task<Response> ProcessCommand()
        {
            if( !IsValid ) throw new InvalidOperationException( "Invalid pipeline state!" );

            object command = await BindCommand().ConfigureAwait( false );
            if( command == null ) return new Responses.InvalidResponse( Context.CommandId, "Command cannot be bind. See logs for details." );

            Response response = await GetMeta( command ).ConfigureAwait( false );
            if( response != null ) return response;

            response = await ApplyFilters( command ).ConfigureAwait( false );
            if( response != null ) return response;

            ICommandReceiver commandReceiver = Services.GetRequiredService<ICommandReceiver>();
            response = await commandReceiver.ReceiveCommand( command, Context ).ConfigureAwait( false );
            if( response != null ) return response;

            return null;
        }


        public Task<object> BindCommand()
        {
            using( Context.Monitor.OpenInfo( $"Binding Command..." ) )
            {
                Type binderType;
                if( Context.Model.Binder != null )
                {
                    Context.Monitor.Trace( "A custom CommandBinder is defined for this command..." );
                    binderType = Context.Model.Binder;
                }
                else
                {
                    binderType = Model.Binder;
                }
                if( binderType == null ) throw new InvalidOperationException( "No command binder found." );

                var binder = Services.GetRequiredService( binderType ) as ICommandBinder;
                if( binder == null )
                {
                    Context.Monitor.Error( $"Unabled to create a command binder of type {binderType}" );
                    return null;
                }
                return binder.Bind( Context );
            }
        }

        public virtual IEnumerable<ICommandFilter> GetFilters() => Services.GetRequiredService<ICommandFilterProvider>().GetFilters( Context, Model );

        public async Task<Response> GetMeta( object command )
        {
            if( command is MetaCommand metaCommand )
            {
                return await MetaCommand.Result.CreateAsync( metaCommand, Model, Services );
            }
            return null;
        }

        public async Task<Response> ApplyFilters( object command )
        {
            if( command == null ) throw new ArgumentNullException( nameof( command ) );

            CommandFilterContext context = new CommandFilterContext( command, Context, Model );
            foreach( var filter in GetFilters() )
            {
                await filter.OnFilterAsync( context ).ConfigureAwait( false );
                if( context.Response != null )
                {
                    return context.Response;
                }
            }
            return null;
        }
    }
}
