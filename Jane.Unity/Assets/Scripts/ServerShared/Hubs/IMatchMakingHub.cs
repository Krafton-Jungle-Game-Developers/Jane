using System;
using System.Threading.Tasks;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IMatchMakingHub : IStreamingHub<IMatchMakingHub, IMatchMakingHubReceiver>
    {
        ValueTask<MatchMakingLobbyUser[]?> EnrollAsync(MatchMakingEnrollRequest request);
        ValueTask ChangeReadyStateAsync(bool isReady);
        ValueTask LeaveAsync();
    }
}