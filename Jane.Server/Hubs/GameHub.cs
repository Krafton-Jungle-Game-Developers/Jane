using System;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using MagicOnion.Server.Hubs;
using UnityEngine;

namespace Jane.Server.Hubs
{
    public class GameHub : StreamingHubBase<IGameHub, IGameHubReceiver>, IGameHub
    {
        private IGroup _game;
        private GamePlayerData self;
        private IInMemoryStorage<GamePlayerData> storage;
        private Ulid _gameId = Ulid.MinValue;
        private bool isJoinGameSuccess = false;

        public async ValueTask<GameJoinResponse> JoinAsync(GameJoinRequest request)
        {
            self = new()
            {
                GameId = _gameId,
                UserId = request.UserId,
                UniqueId = request.UniqueId,
                Position = request.InitialPosition,
                Rotation = request.InitialRotation
            };

            (isJoinGameSuccess, _game, storage) = await Group.TryAddAsync(_gameId.ToString(),
                4,
                true,
                self);

            if (isJoinGameSuccess is false) { return new() { GameId = Ulid.Empty, Players = null }; }

            int userCount = await _game.GetMemberCountAsync();
            
            Vector3 pos = new(request.InitialPosition.x + (20 * int.Clamp(userCount - 1, 0, 3)),
                              request.InitialPosition.y,
                              request.InitialPosition.z);

            self.Position = pos;

            Broadcast(_game).OnJoin(self);

            return new() { GameId = _gameId, Players = storage.AllValues.ToArray() };
        }

        public async ValueTask LeaveAsync()
        {
            Broadcast(_game).OnLeave(self);
            await _game.RemoveAsync(Context);
        }

        public ValueTask MoveAsync(MoveRequest request)
        {
            self.Position = request.Position;
            self.Rotation = request.Rotation;
            BroadcastExceptSelf(_game).OnMove(request);
            return CompletedTask;
        }

        protected override ValueTask OnDisconnected()
        {
            BroadcastExceptSelf(_game).OnLeave(self);
            return CompletedTask;
        }
    }
}
