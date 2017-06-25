using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class CommandResponseBuilder
    {
        Func<CommandResponse, Stream, Task> _writeDelegate;
        CommandResponse _response;

        public CommandResponseBuilder()
        {
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Extra informations returned by Crs.
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        public async Task<bool> WriteAsync( Stream output )
        {
            if (_writeDelegate != null && _response != null)
            {
                await _writeDelegate.Invoke(_response, output);
                return true;
            }
            return false;
        }

        public void RegisterWriteHandler(Func<CommandResponse, Stream, Task> writeDelegate)
        {
            _writeDelegate = writeDelegate;
        }

        /// <summary>
        /// Gets the response. Can be null.
        /// </summary>
        public CommandResponse CommandResponse => _response;

        public bool HasReponse => _response != null;

        public void Set( CommandResponse response )
        {
            _response = response;
        }
    }
}
