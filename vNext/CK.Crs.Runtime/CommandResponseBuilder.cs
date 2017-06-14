using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class CommandResponseBuilder
    {
        Func<CommandResponse, Stream, Task> _outputDelegate;
        CommandResponse _response;

        public CommandResponseBuilder()
        {
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Extra informations returned by Crs.
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        public async Task<bool> BuildAsync( Stream output )
        {
            if (_outputDelegate != null && _response != null)
            {
                await _outputDelegate.Invoke(_response, output);
                return true;
            }
            return false;
        }

        public void RegisterOutputHandler(Func<CommandResponse, Stream, Task> outputDelegate)
        {
            _outputDelegate = outputDelegate;
        }

        public bool HasReponse => _response != null;

        public void Set( CommandResponse response )
        {
            _response = response;
        }
    }
}
