using System;
using Jane.Unity.ServerShared.Enums;
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

        // TODO: PlayerCount of a game should never be received from client. Anti-pattern.
        private int totalPlayerCount;
        
        private IInMemoryStorage<GamePlayerData> storage;
        private GamePlayerData self;

        private Ulid gameId;
        private GameState gameState = GameState.Waiting;
        private TimeSpan gameDuration = TimeSpan.FromSeconds(30);
        private TimeSpan timeLeft;
        private Task waitOtherPlayersTask = null;
        private Task timerTask = null;
        private CancellationTokenSource cts = new();

        public async ValueTask<GameJoinResponse> JoinAsync(GameJoinRequest request)
        {
            self = new()
            {
                GameId = request.GameId,
                UserId = request.UserId,
                UniqueId = request.UniqueId,
                IsInitialized = request.IsInitialized,
                Position = request.InitialPosition,
                Rotation = request.InitialRotation,
                CurrentRegion = 1,
                CurrentZone = 0,
                HP = 20
            };

            totalPlayerCount = request.PlayerCount;

            (isJoinGameSuccess, game, storage) = await Group.TryAddAsync(self.GameId.ToString(),
                totalPlayerCount,
                true,
                self);

            if (isJoinGameSuccess is false) { return new() { GameId = Ulid.Empty, Players = null }; }
            gameId = self.GameId;

            int userCount = await game.GetMemberCountAsync();
            if (waitOtherPlayersTask is null)
            {
                waitOtherPlayersTask = Task.Run(async () =>
                {
                    while (storage.AllValues.Any(user => user.IsInitialized is false)) { await Task.Delay(1000); }

                    GameStateChangedResponse countDown = new() { GameId = gameId, GameState = GameState.CountDown };
                    Broadcast(game).OnGameStateChange(countDown);

                    await Task.Delay(3000);

                    GameStateChangedResponse gameStart = new() { GameId = gameId, GameState = GameState.Playing };
                    Broadcast(game).OnGameStateChange(gameStart);

                    if (timerTask is null)
                    {
                        timerTask = Task.Run(async () =>
                        {
                            TimeSpan timeLeft = gameDuration;
                            TimeSpan checkInterval = TimeSpan.FromMilliseconds(10);

                            while (timeLeft > TimeSpan.Zero)
                            {
                                await Task.Delay(checkInterval);
                                timeLeft -= checkInterval;

                                Broadcast(game).OnTimerUpdate(timeLeft.Ticks);
                            }

                            GameStateChangedResponse gameEnd = new() { GameId = gameId, GameState = GameState.Finished };
                            Broadcast(game).OnGameStateChange(gameEnd);
                        });
                    }
                });
            }

            Vector3 pos = new(request.InitialPosition.x + (20 * int.Clamp(userCount - 1, 0, 3)),
                              request.InitialPosition.y,
                              request.InitialPosition.z);

            self.Position = pos;

            Broadcast(game).OnJoin(self);
            return new() { GameId = gameId, Players = storage.AllValues.ToArray() };
        }

        public ValueTask GameInitializedAsync(GameInitializedRequest request)
        {
            self.IsInitialized = true;
            return CompletedTask;
        }

        // TODO: When Game Starts, Timer of 120 seconds start.
        // TODO: From client side, await for Server's Timer start response and start 120 second timer instantly.
        // No matter what (even in Pause Menu), timer always runs.
        // TODO: When Server Timer reaches 0, Broadcast Game End.

        // TODO: On Every Frame while in GameState.Playing,
        // BroadCast Player's Current Region and Rank
        // Rank is based on distance between Region Start and Player Position)

        // TODO: If any player enters Region N, Activate MeteorSpawner(s) of that Region. (Dependent to Region.)
        // Spawned Meteor(s) cannot go further than that Region.

        // TODO: When a meteor is spawned, it should be in ConcurrentDictionary.
        // If a player collides to a meteor, HP should decrease, and meteor should be removed.
        // Meteor can collide to: Bullet, Player, Environment
        // When a collision happens, it is removed and broadcasted.

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
