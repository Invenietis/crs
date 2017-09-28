using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Infrastructure
{

    class CrsReceiverModel : IEndpointModel
    {
        private Type _currentConfiguredEndpoint;
        private IEnumerable<CommandModel> _requests;
        private readonly ICrsModel _model;

        public CrsReceiverModel( ICrsModel model, Type currentConfiguredEndpoint, IEnumerable<CommandModel> requests )
        {
            _model = model ?? throw new ArgumentNullException( nameof( model ) );
            _currentConfiguredEndpoint = currentConfiguredEndpoint ?? throw new ArgumentNullException( nameof( currentConfiguredEndpoint ) );
            _requests = requests;
        }
        public ICrsModel CrsModel => _model;

        public string Name => _currentConfiguredEndpoint.Name;

        public Type EndpointType => _currentConfiguredEndpoint;

        public string CallerIdName { get; set; } = "CallerId";

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        public bool ApplyAmbientValuesValidation { get; internal set; } = true;

        public bool ApplyModelValidation { get; internal set; } = true;

        public IEnumerable<CommandModel> Commands => _requests;


        public CommandModel GetCommandModel( Type requestType )
        {
            // TODO: lookup in a dictionary ?
            // TODO: why first or default? We must ensure that we get the good one.
            // Maybe by adding a CKTrait during configuration to each RequestDescription. This 
            // traits will identityf the Receiver (which is unique)
            // BTW, We should decouple which request are handle by which receiver...
            // The request registry and the requests by receivers which is a subset of the registry.
            return Commands.FirstOrDefault( t => t.CommandType == requestType );
        }
    }
}
