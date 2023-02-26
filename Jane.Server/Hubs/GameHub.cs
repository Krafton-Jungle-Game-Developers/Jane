using System;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion.Server.Hubs;
using UnityEngine;

namespace Jane.Server.Hubs
{
    public class GameHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        private IGroup room;
        private Player self;
        private IInMemoryStorage<Player> storage;

        public async ValueTask<Player[]> JoinAsync(Ulid roomId, Ulid userId, Vector3 position, Quaternion rotation)
        {
            self = new() { Id = userId, Position = position, Rotation = rotation };
            (room, storage) = await Group.AddAsync(roomId.ToString(), self);

            Broadcast(room).OnJoin(self);

            return storage.AllValues.ToArray();
        }

        public async ValueTask LeaveAsync()
        {
            await room.RemoveAsync(Context);
            Broadcast(room).OnLeave(self);
        }

        public ValueTask MoveAsync(MoveRequest request)
        {
            self.Position = request.Position;
            self.Rotation = request.Rotation;
            Broadcast(room).OnMove(request);
            return CompletedTask;
        }

        protected override ValueTask OnDisconnected()
        {
            BroadcastExceptSelf(room).OnLeave(self);
            return CompletedTask;
        }
    }
}
