using System;
using System.Threading.Tasks;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion;
using UnityEngine;

namespace Jane.Unity.ServerShared.Hubs
{
    public interface IMovementHub : IStreamingHub<IMovementHub, IMovementHubReceiver>
    {
        ValueTask<MoveRequest[]> JoinAsync(Ulid roomId, Ulid userId, Vector3 position, Quaternion rotation);
        ValueTask LeaveAsync();
        ValueTask MoveAsync(Vector3 position, Quaternion rotation);
    }
}
