using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    interface ICrsHub
    {
        Task ReceiveResult( string message );

        Task ReceiveCallerId( string callerId );
    }
}
