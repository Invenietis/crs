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
        public int IDatabase { get; private set; }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddCommandReceiver( options =>
            {
                options.Registry.EnableLongRunningCommands = false;
                options.Registry.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>().CommandName( "transfer" ).IsLongRunning();
                options.Registry.Register<WithdrawMoneyCommand, WithDrawyMoneyHandler>().CommandName( "withdraw" ).AddDecorator<TransactionAttribute>();
                options.Registry.Register<UserCommand, UserHandler>().CommandName( "addUser" ).AddDecorator<TransactionAttribute>();

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
            } );

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
                    .AddFilter<CK.Crs.ProtectedResourceAuthorizationFilter>()
                    .AddCommands(
                        registry => registry.Registration.Where( c => c.CommandType.Namespace.StartsWith( "CK.Crs" ) ),
                        config => config.AddExtraData( "Permission", CK.Authorization.MinGrantLevel.Administrator ) )
                    .AddCommand<TransferAmountCommand>().CommandName( "transfer" ).AddDecorator<TransactionAttribute>();
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
                        .UseCommandExecutor();
                // Not default handler.
                //.UseSignalRDispatcher();
            } );

            SimpleServiceContainer simpleServiceContainer = new SimpleServiceContainer( app.ApplicationServices );

            //app.UseOwin( pipeline =>
            //{
            //    pipeline.UseBuilder( simpleServiceContainer );
            //} );

            app.UseOwin( pipeline =>
            {
                var receiver = simpleServiceContainer.GetRequiredService<ICommandReceiver>();
                var middleWare = new CommandReceiverOwinMiddleware( receiver );
                pipeline( _ =>
                {
                    return middleWare.InvokeAsync;
                } );
            } );

            //options.OnCommandEvent( x => Azure.Publish );
            app.UseMvc();

        }
    }
}
