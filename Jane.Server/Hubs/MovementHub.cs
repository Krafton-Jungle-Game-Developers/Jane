using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion.Server.Hubs;
using UnityEngine;

namespace Jane.Server.Hubs
{
    public class MovementHub : StreamingHubBase<IMovementHub, IMovementHubReceiver>, IMovementHub
    {
        public ValueTask<MoveRequest[]> JoinAsync(Ulid roomId, Ulid userId, Vector3 position, Quaternion rotation)
        {
            throw new NotImplementedException();
        }

        public ValueTask LeaveAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask MoveAsync(Vector3 position, Quaternion rotation)
        {
            throw new NotImplementedException();
        }
    }
}
