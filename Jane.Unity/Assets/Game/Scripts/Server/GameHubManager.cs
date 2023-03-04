using System;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;

using MagicOnion;
using MagicOnion.Client;

using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;

public class GameHubManager : MonoBehaviour, IGameHubReceiver
{
    private readonly CancellationTokenSource shutdownCts = new();
    private GrpcChannelManager channelManager;
    private IGameHub? gameHub;

    private Dictionary<Ulid, NetworkPlayer> players = new(4);
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject otherPrefab;
    private NetworkPlayer _self;
    [Header("Player Reference Init")]
    [SerializeField] private SpaceshipInputManager inputManager;
    [SerializeField] private SpaceshipCameraController cameraController;

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

        _self = await ConnectAsync();

        // TODO: 이동 가능 상태 세분화
        // TODO: Lag Inspection
        await UniTaskAsyncEnumerable.IntervalFrame(1)
            .Where(_ => _self is not null && gameHub is not null)
            .ForEachAwaitAsync(async _ =>
            {
                await gameHub.MoveAsync(new()
                {
                    Id = _self.UniqueId,
                    Position = _self.transform.position,
                    Rotation = _self.transform.rotation
                });
            }, shutdownCts.Token);
    }

    public async UniTask<NetworkPlayer> ConnectAsync()
    {
        GameJoinRequest request = new()
        {
            GameId = GameInfo.GameId,
            UserId = UserInfo.UserId,
            UniqueId = UserInfo.UniqueId,
            InitialPosition = Vector3.zero,
            InitialRotation = Quaternion.identity
        };

        GameJoinResponse response;
        try
        {
            response = await gameHub.JoinAsync(request).AsTask().AsUniTask();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
        
        GamePlayerData[] roomPlayers = response.Players;

        foreach (GamePlayerData player in roomPlayers)
        {
            Debug.Log($"Player:{player.UserId}, UID:{player.UniqueId}");
            if (player.UniqueId.Equals(UserInfo.UniqueId)) { continue; }

            OnJoin(player);
        }

        return players[UserInfo.UniqueId];
    }

    public void OnJoin(GamePlayerData joinedPlayer)
    {
        if (joinedPlayer is null) { throw new ArgumentNullException(); }

        Debug.Log($"Player {joinedPlayer.UserId} has joined the room.");

        GameObject playerGameObject;
        NetworkPlayer networkPlayer;

        if (joinedPlayer.UniqueId.Equals(UserInfo.UniqueId))
        {
            playerGameObject = Instantiate(playerPrefab, joinedPlayer.Position, joinedPlayer.Rotation);
            cameraController.CurrentViewTarget = playerGameObject.transform.Find("Camera Target");
            cameraController.SpaceShipRigidbody = playerGameObject.GetComponent<Rigidbody>();
            cameraController.SpaceEngine = playerGameObject.GetComponent<SpaceshipEngine>();
            
            inputManager.SpaceEngine = playerGameObject.GetComponent<SpaceshipEngine>();
            inputManager.HUDCursor = playerGameObject.GetComponentInChildren<HUDCursor>();
            inputManager.HUDCursor.HUDCamera = cameraController.MainCamera;
            inputManager.EnableInput();
            inputManager.EnableSteering();
            inputManager.EnableMovement();
        }
        else
        {
            playerGameObject = Instantiate(otherPrefab, joinedPlayer.Position, joinedPlayer.Rotation);
        }

        playerGameObject.name = joinedPlayer.UserId;

        networkPlayer = playerGameObject.GetComponent<NetworkPlayer>();
        networkPlayer.Initialize(joinedPlayer);

        players.TryAdd(joinedPlayer.UniqueId, networkPlayer);
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

    public void OnMove(MoveRequest request)
    {
        if (players.TryGetValue(request.Id, out NetworkPlayer other))
        {
            other.UpdateMovement(request);
        }
    }
}
