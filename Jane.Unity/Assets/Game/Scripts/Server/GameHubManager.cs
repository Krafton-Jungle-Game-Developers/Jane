using System;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

using MagicOnion;
using MagicOnion.Client;

using Jane.Unity;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.Enums;
using Jane.Unity.ServerShared.MemoryPackObjects;

namespace Jane.Unity.Server
{
    public class GameHubManager : MonoBehaviour, IGameHubReceiver
    {
        private readonly CancellationTokenSource shutdownCts = new();
        private readonly CancellationTokenSource gameCts = new();
        private GrpcChannelManager channelManager;
        private IGameHub? gameHub;

        private NetworkPlayer self;
        private Dictionary<Ulid, NetworkPlayer> players = new(4);
        [SerializeField] private GameObject playerGameObject;
        [SerializeField] private GameObject otherPlayerPrefab;
        [SerializeField] private GameManager gameManager;
        
        private void OnEnable()
        {
            channelManager = FindObjectOfType<GrpcChannelManager>();
        }

        private async UniTaskVoid OnDestroy()
        {
            shutdownCts.Cancel();

            if (gameHub is not null)
            {
                await gameHub.LeaveAsync().AsTask().AsUniTask();
                await gameHub.DisposeAsync().AsUniTask();
            }
        }

        public async UniTask InitializeAsync()
        {
            while (!shutdownCts.IsCancellationRequested)
            {
                try
                {
                    Debug.Log("Connecting Server from GameHubManager...");
                    gameHub = await StreamingHubClient.ConnectAsync<IGameHub, IGameHubReceiver>(channelManager.GRPCChannel,
                                                                                        this,
                                                                                                cancellationToken: shutdownCts.Token).AsUniTask();
                    Debug.Log("GameHub connection established!");
                    break;
                }
                catch (Exception e) { Debug.LogError(e); }

                Debug.Log($"Failed to connect to the server. Retrying after 5 seconds");
                await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: shutdownCts.Token);
            }

            await JoinAsync();

            WaitForOtherPlayersReadyAsync(shutdownCts.Token).Forget();
        }

        private async UniTask WaitForOtherPlayersReadyAsync(CancellationToken token)
        {
            await UniTask.WaitUntil(() => players.Count == GameInfo.PlayerCount, cancellationToken: token);

            if (token.IsCancellationRequested) { return; }

            GameInitializedRequest request = new() { UniqueId = UserInfo.UniqueId };
            await gameHub.GameInitializedAsync(request).AsTask().AsUniTask();

            // TODO: UI - Waiting for other players
            await UniTask.WaitUntil(() => GameInfo.GameState is GameState.CountDown, cancellationToken: token);

            // TODO: UI - CountDown
            WaitForGameStartAsync(token).Forget();
        }

        private async UniTask WaitForGameStartAsync(CancellationToken token)
        {
            await gameManager.CountDownAsync(3);
            await UniTask.WaitUntil(() => GameInfo.GameState is GameState.Playing, cancellationToken: token);

            gameManager.StartGame().Forget();

            WaitForGameOverAsync(token).Forget();
        }

        private async UniTask WaitForGameOverAsync(CancellationToken token)
        {
            UniTaskAsyncEnumerable.IntervalFrame(1)
                .Where(_ => GameInfo.GameState is GameState.Playing)
                .ForEachAsync(_ => gameHub.MoveAsync(new()
                {
                    Id = UserInfo.UniqueId,
                    Position = self.transform.position,
                    Rotation = self.transform.rotation
                }), gameCts.Token).SuppressCancellationThrow().Forget();


            await UniTask.WaitUntil(() => GameInfo.GameState is GameState.GameOver, cancellationToken: token);
            gameCts.Cancel();
            
            gameManager.GameOver();

            Debug.Log("Game Over!");
        }

        public async UniTask JoinAsync()
        {
            GameJoinRequest request = new()
            {
                GameId = GameInfo.GameId,
                PlayerCount = GameInfo.PlayerCount,
                UserId = UserInfo.UserId,
                UniqueId = UserInfo.UniqueId,
                IsInitialized = false,
                InitialPosition = Vector3.zero,
                InitialRotation = Quaternion.identity
            };

            GameJoinResponse response;
            try
            {
                // TODO: When Response.GameId is Ulid.Empty, GameJoin has failed.
                response = await gameHub.JoinAsync(request).AsTask().AsUniTask();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
            
            foreach (GamePlayerData player in response.Players)
            {
                if (player.UniqueId.Equals(UserInfo.UniqueId)) { continue; }

                OnJoin(player);
            }
        }
        
        public void OnJoin(GamePlayerData joinedPlayerData)
        {
            if (joinedPlayerData is null) { throw new ArgumentNullException(); }

            Debug.Log($"Player {joinedPlayerData.UserId}: {joinedPlayerData.UniqueId} has joined the room.");
            
            NetworkPlayer networkPlayer;

            // ME: Server sent that I've successfully joined the room. So I should Initialize Player.
            if (joinedPlayerData.UniqueId.Equals(UserInfo.UniqueId))
            {
                networkPlayer = playerGameObject.GetComponent<NetworkPlayer>();
                networkPlayer.Initialize(joinedPlayerData, true);
                self = networkPlayer;
                // Enable Input when game starts
                // Call MoveAsync Every frame
            }
            else
            {
                GameObject other = Instantiate(otherPlayerPrefab, joinedPlayerData.Position, joinedPlayerData.Rotation);
                networkPlayer = other.GetComponent<NetworkPlayer>();
                networkPlayer.Initialize(joinedPlayerData, false);
            }
            
            players.TryAdd(joinedPlayerData.UniqueId, networkPlayer);
        }

        public void OnLeave(GamePlayerData request)
        {
            Debug.Log($"Player {request.UserId} has left the room.");

            if (players.TryGetValue(request.UniqueId, out NetworkPlayer other))
            {
                players.Remove(request.UniqueId);
                other.OnLeaveRoom();
            }
        }

        public void OnGameStateChange(GameStateChangedResponse response)
        {
            GameInfo.GameState = response.GameState;
        }

        public void OnTimerUpdate(long ticks)
        {
            gameManager.UpdateTimer(ticks);
        }

        public void OnMove(MoveRequest request)
        {
            if (players.TryGetValue(request.Id, out NetworkPlayer other))
            {
                other.UpdateMovement(request);
            }
        }
    }
}
