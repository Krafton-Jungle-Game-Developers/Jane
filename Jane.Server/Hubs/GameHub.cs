using System;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion.Server.Hubs;
using UnityEngine;

namespace Jane.Server.Hubs
{
    public class GameHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        private bool isJoinGameSuccess = false;
        private IGroup game;
        private IInMemoryStorage<GamePlayerData> storage;
        private GamePlayerData self;

        private Ulid gameId;

        public async ValueTask<GameJoinResponse> JoinAsync(GameJoinRequest request)
        {
            self = new()
            {
                GameId = request.GameId,
                UserId = request.UserId,
                UniqueId = request.UniqueId,
                Position = request.InitialPosition,
                Rotation = request.InitialRotation
            };

            (isJoinGameSuccess, game, storage) = await Group.TryAddAsync(self.GameId.ToString(),
                4,
                true,
                self);

            if (isJoinGameSuccess is false) { return new() { GameId = Ulid.Empty, Players = null }; }
            gameId = self.GameId;

            int userCount = await game.GetMemberCountAsync();
            
            Vector3 pos = new(request.InitialPosition.x + (20 * int.Clamp(userCount - 1, 0, 3)),
                              request.InitialPosition.y,
                              request.InitialPosition.z);

            self.Position = pos;

            Broadcast(game).OnJoin(self);

            return new() { GameId = gameId, Players = storage.AllValues.ToArray() };
        }

        public async ValueTask LeaveAsync()
        {
            Broadcast(game).OnLeave(self);
            await game.RemoveAsync(Context);
        }

        public ValueTask MoveAsync(MoveRequest request)
        {
            self.Position = request.Position;
            self.Rotation = request.Rotation;
            BroadcastExceptSelf(game).OnMove(request);
            return CompletedTask;
        }

        protected override ValueTask OnDisconnected()
        {
            BroadcastExceptSelf(game).OnLeave(self);
            return CompletedTask;
        }
    }
}
