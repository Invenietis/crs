using CK.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class HttpCrsEndpointPipeline : ICrsEndpointPipeline
    {
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

        public async Task<Response> ProcessCommand()
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

        public Task<object> BindCommand()
        {
            var binder = ActivatorUtilities.CreateInstance(
                HttpContext.RequestServices,
                Context.Model.Binder ?? Model.Binder,
                HttpContext ) as ICommandBinder;

            return binder.Bind( Context );
        }

        public async Task<Response> ApplyFilters( object command )
        {
            if( command == null )
            {
                throw new ArgumentNullException( nameof( command ) );
            }

            if( Model.ApplyAmbientValuesValidation )
            {
                IAmbientValues ambientValues = HttpContext.RequestServices.GetRequiredService<IAmbientValues>();
                IAmbientValuesRegistration ambientValueRegistration = HttpContext.RequestServices.GetRequiredService<IAmbientValuesRegistration>();
                var vContext = new ReflectionAmbientValueValidationContext( command, Monitor, ambientValues );

                foreach( var v in ambientValueRegistration.AmbientValues )
                {
                    await vContext.ValidateValueAndRejectOnError( v.Name );
                }

                if( vContext.Rejected )
                {
                    return new InvalidResponse( Context.CommandId, vContext.RejectReason );
                }
            }
            if( Model.ApplyModelValidation )
            {
                var items = new Dictionary<object, object>();
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext( command, HttpContext.RequestServices, items );
                if( !Validator.TryValidateObject( command, validationContext, validationResults ) )
                {
                    return new InvalidResponse( Context.CommandId, validationResults );
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
