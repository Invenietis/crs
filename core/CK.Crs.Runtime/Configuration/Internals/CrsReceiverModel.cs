using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Configuration
{

    class CrsReceiverModel : IEndpointModel
    {
        private IEnumerable<CommandModel> _requests;
        private readonly ICrsModel _model;
        private readonly string _path;

        public CrsReceiverModel( string path, ICrsModel model, Type commandBinder, IEnumerable<CommandModel> requests )
        {
            _model = model ?? throw new ArgumentNullException( nameof( model ) );
            _path = path ?? throw new ArgumentNullException( nameof( path ) );
            Binder = commandBinder;
            _requests = requests;
        }

        public ICrsModel CrsModel => _model;

        public string Path => _path;

        public Type Binder { get; }

        public IResponseFormatter ResponseFormatter { get; set; }

        public string CallerIdName { get; set; }

        /// <summary>
        /// Gets wether we should validate ambient values or not.
        /// </summary>
        public bool ApplyAmbientValuesValidation { get; set; }

        public bool ApplyModelValidation { get; set; }

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
