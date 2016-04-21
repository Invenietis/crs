using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using CK.Crs.Handlers;
using System.Threading.Tasks;
using System;
using CK.Crs;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Tests.Integration
{
    public class Startup
    {
        class SimpleActorIdProvider : IAmbientValueProvider
        {
            public Task<object> GetValueAsync( IAmbientValues values )
            {
                return Task.FromResult<object>( null );
            }
        }
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddCommandReceiver( config =>
            {
                config.Registry.EnableLongRunningCommands = false;
                config.Registry.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>().CommandName( "transfer" );
                config.Registry.Register<WithdrawMoneyCommand, WithDrawyMoneyHandler>().CommandName( "withdraw" ); //.AddDecorator<TransactionAttribute>();
                config.Registry.Register<UserCommand, UserHandler>().CommandName( "addUser" ); //.AddDecorator<TransactionAttribute>();

                config.AmbientValues.Register<SimpleActorIdProvider>( "ActorId" );
                config.AmbientValues.RegisterValue( "AuthenticatedActorId", 0 );
            } );

            services.AddCommandExecutor();

            services.AddMvc();
            services.AddSingleton<IRepository<UserModel>, UserRepository>();
            services.AddTransient<UserHandler>();
        }

        public void Configure( IApplicationBuilder app )
        {
            app.UseStaticFiles();
            app.UseCommandReceiver( "/c/admin", options =>
            {
                options
                    .AddFilter<HttpsRequiredFilter>()
                    //.AddFilter<CK.Crs.ProtectedResourceAuthorizationFilter>()
                    //.AddCommands(
                    //    registry => registry.Registration.Where( c => c.CommandType.Namespace.StartsWith( "CK.Crs" ) ),
                    //    config => config.AddExtraData( "Permission", Authorization.MinGrantLevel.Administrator ) )
                    .AddCommand<TransferAmountCommand>().CommandName( "transfer" ).IsAsync(); //.AddDecorator<TransactionAttribute>();

                options.Events.AmbientValuesValidating = async validationContext =>
                {
                    int actorId = await validationContext.AmbientValues.GetValueAsync<int>("ActorId");

                    await validationContext.ValidateValueAndRejectOnError( "TenantId", new AmbientValueComparer<int>( ( valueName, commandValue, ambientValue ) =>
                    {
                        bool tenantIsTheSame = commandValue == ambientValue;

                        if( !tenantIsTheSame )
                        {
                            if( actorId == 1 ) return true;
                        }

                        return tenantIsTheSame;
                    } ) );
                };

                options.Pipeline.Clear().UseDefault().UseSyncCommandExecutor().UseJsonCommandWriter();
            } );


            app.UseCommandReceiver( "/c/public", options =>
            {
                options
                    .Pipeline
                        .UseDefault();
                //.UseSignalRDispatcher();
            } );

            app.UseCommandReceiver( "/c/public", options =>
            {
                options
                    .Pipeline
                        // These handlers are defaults handlers
                        .UseCommandRouter()
                        .UseJsonCommandBuilder()
                        .UseAmbientValuesValidator()
                        .UseFilters()
                        .UseSyncCommandExecutor()
                        .UseJsonCommandWriter();

                options.Events.CommandRejected = context =>
                {
                    if( context.Action.Description.Descriptor.Name == "Logout" )
                    {
                        // Never reject logout command for any reason :p
                        context.CancelRejection();
                    }
                    return Task.FromResult( 0 );
                };

                options.Events.CommandExecuting = context =>
                {
                    context.SetResult( null );
                    return Task.FromResult( 0 );
                };
                // Not default handler.
                //.UseSignalRDispatcher();
            } );


            //app.UseOwin( pipeline =>
            //{
            //    pipeline.UseBuilder( simpleServiceContainer );
            //} );

            //SimpleServiceContainer services = new SimpleServiceContainer( app.ApplicationServices );
            //app.UseOwin( pipeline =>
            //{
            //    var registry = services.GetService( typeof( ICommandRegistry ) ) as ICommandRegistry;
            //    var events = services.GetService( typeof( PipelineEvents ) ) as PipelineEvents;
            //    var routeCollection = new CommandRouteCollection( routePrefix.Value );
            //    var middlewareConfiguration = new CommandReceiverConfiguration( registry, routeCollection, services );
            //    config( middlewareConfiguration );

            //    var commandReceiver = new CommandReceiver( services, middlewareConfiguration.Pipeline, events, routeCollection);
            //    var middleWare = new CommandReceiverOwinMiddleware( receiver );
            //    pipeline( _ =>
            //    {
            //        return middleWare.InvokeAsync;
            //    } );
            //} );

            //options.OnCommandEvent( x => Azure.Publish );
            app.UseMvc();

        }
    }
}
