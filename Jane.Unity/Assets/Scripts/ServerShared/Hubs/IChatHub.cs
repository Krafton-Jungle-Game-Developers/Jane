using MagicOnion;
using System.Threading.Tasks;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IChatHub : IStreamingHub<IChatHub, IChatHubReceiver>
    {
        ValueTask JoinAsync(string roomName, string userName);
        ValueTask LeaveAsync();
        ValueTask SendMessageAsync(string message);
    }
}
