using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    interface ICrsHub
    {
        Task ReceiveResult( string commandId, char responseType, object payload );

        Task ReceiveCallerId( string callerId );
    }
}
