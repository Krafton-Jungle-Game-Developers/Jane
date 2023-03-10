using System;
using System.Threading.Tasks;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion;
using UnityEngine;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IGameHub : IStreamingHub<IGameHub, IGameHubReceiver>
    {
        ValueTask<GameJoinResponse> JoinAsync(GameJoinRequest request);
        ValueTask GameInitializedAsync(GameInitializedRequest request);
        ValueTask LeaveAsync();
        ValueTask MoveAsync(MoveRequest request);
    }
}
