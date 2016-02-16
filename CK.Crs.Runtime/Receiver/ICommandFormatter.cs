using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandFormatter
    {
        CommandRequest Deserialize( RoutedCommandDescriptor commandDescription, Stream inputStream, IDictionary<string,object> metadata );

        void Serialize( CommandResponse response, Stream outputStream );
    }
}
