using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    public interface ICrsHub
    {
        Task ReceiveResult( string message );

        Task ReceiveCallerId( string callerId );
    }
}
