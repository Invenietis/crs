using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Crs.Configuration
{

    class CrsReceiverModel : IEndpointModel
    {
        private IEnumerable<ICommandModel> _commands;
        private readonly ICrsModel _model;
        private readonly string _path;

        public CrsReceiverModel( string path, ICrsModel model, ICommandBinder commandBinder, IEnumerable<ICommandModel> commands )
        {
            _model = model ?? throw new ArgumentNullException( nameof( model ) );
            _path = path ?? throw new ArgumentNullException( nameof( path ) );
            _commands = commands;
            Binder = commandBinder;
            Filters = Type.EmptyTypes;
        }

        public ICrsModel CrsModel => _model;

        public string Path => _path;

        public ICommandBinder Binder { get; }

        public IEnumerable<Type> Filters { get; private set; }

        public IResponseFormatter ResponseFormatter { get; set; }

        public string CallerIdName { get; set; }

        public IEnumerable<ICommandModel> Commands => _commands;

        public ICommandModel GetCommandModel( Type requestType )
        {
            // TODO: lookup in a dictionary ?
            // TODO: why first or default? We must ensure that we get the good one.
            // Maybe by adding a CKTrait during configuration to each RequestDescription. This 
            // tags will identityf the Receiver (which is unique)
            // BTW, We should decouple which request are handle by which receiver...
            // The request registry and the requests by receivers which is a subset of the registry.
            return Commands.FirstOrDefault( t => t.CommandType == requestType );
        }

        internal void AddFilter( Type filterType ) => Filters = Filters.Concat( new[] { filterType } );
    }
}
