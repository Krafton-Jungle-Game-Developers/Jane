using System;
using System.Threading.Tasks;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion;
using UnityEngine;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IGameHub : IStreamingHub<IGameHub, IGameHubReceiver>
    {
        ValueTask<Player[]> JoinAsync(Ulid roomId, Ulid userId, Vector3 position, Quaternion rotation);
        ValueTask LeaveAsync();
        ValueTask MoveAsync(MoveRequest request);
    }
}
