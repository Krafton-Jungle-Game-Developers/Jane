using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using MagicOnion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;
using UnityEngine;

public class GameHubManager : MonoBehaviour, IGameHubReceiver
{
    private Dictionary<Ulid, NetworkPlayer> players = new(4);
    private readonly CancellationTokenSource _shutdownCts = new();
    private ChannelBase _channel;
    private IGameHub gameHub;
    
    private Ulid myUniqueId;
    private Ulid gameId;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject otherPrefab;
    private NetworkPlayer _self;
    [Header("Player Reference Init")]
    [SerializeField] private SpaceshipInputManager inputManager;
    [SerializeField] private SpaceshipCameraController cameraController;


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private async UniTaskVoid Start()
    {
        await InitializeClient(_shutdownCts.Token);
        Debug.Log("Finished Init");
        await UniTaskAsyncEnumerable.IntervalFrame(1)
                                    .Where(_ => _self is not null && gameHub is not null)
                                    .ForEachAwaitAsync(async _ =>
                                    {
                                        await gameHub.MoveAsync(new()
                                        {
                                            Id = myUniqueId,
                                            Position = _self.transform.position,
                                            Rotation = _self.transform.rotation
                                        });
                                    }, _shutdownCts.Token);
    }

    private async UniTaskVoid OnDestroy()
    {
        _shutdownCts.Cancel();

        if (gameHub is not null)
        {
            await gameHub.LeaveAsync().AsTask().AsUniTask();
            await gameHub.DisposeAsync().AsUniTask();
        }
        if (_channel is not null) { await _channel.ShutdownAsync().AsUniTask(); }
    }

    public async UniTask InitializeClient(CancellationToken token)
    {
        _channel = GrpcChannelx.ForTarget(new("jane.jungle-gamedev.com", 5001, false));
        myUniqueId = Ulid.NewUlid();
        while (!_shutdownCts.IsCancellationRequested)
        {
            try
            {
                Debug.Log("Connecting to the server...");
                gameHub = await StreamingHubClient.ConnectAsync<IGameHub, IGameHubReceiver>(_channel, this, cancellationToken: token).AsUniTask();
                
                Debug.Log("Connection established!");
                break;
            }
            catch (Exception e) { Debug.LogError(e); }

            Debug.Log($"Failed to connect to the server. Retrying after 5 seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: token);
        }
        _self = await ConnectAsync();
    }

    public async UniTask<NetworkPlayer> ConnectAsync()
    {
        GameJoinRequest request = new()
        {
            UserId = UserInfo.UserId,
            UniqueId = myUniqueId,
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

        gameId = response.GameId;
        GamePlayerData[] roomPlayers = response.Players;

        foreach (GamePlayerData player in roomPlayers)
        {
            Debug.Log($"Player:{player.UserId}, UID:{player.UniqueId}");
            if (player.UniqueId.Equals(myUniqueId)) { continue; }

            OnJoin(player);
        }

        return players[myUniqueId];
    }

    public void OnJoin(GamePlayerData joinedPlayer)
    {
        if (joinedPlayer is null) { throw new ArgumentNullException(); }

        Debug.Log($"Player {joinedPlayer.UserId} has joined the room.");

        GameObject playerGameObject;
        NetworkPlayer networkPlayer;

        if (joinedPlayer.UniqueId.Equals(myUniqueId))
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
