using System;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using MagicOnion;
using MagicOnion.Client;
using Grpc.Core;
using UnityEngine;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;

public class MoveSyncComponent : MonoBehaviour, IGameHubReceiver
{
    private Dictionary<Ulid, GameObject> players = new();
    private readonly CancellationTokenSource _shutdownCts = new();
    private ChannelBase _channel;
    private IGameHub streamingClient;

    private Ulid _roomId = Ulid.MinValue;
    private Ulid _userId;

    [SerializeField] private GameObject playerPrefab;
    private GameObject _self;

    private void Awake() => Application.targetFrameRate = 144;

    private async UniTaskVoid Start()
    {
        await InitializeClient(_shutdownCts.Token);
        await UniTaskAsyncEnumerable.IntervalFrame(1)
                                    .Where(_ => _self is not null && streamingClient is not null)
                                    .ForEachAwaitAsync(async _ =>
                                    {
                                        await streamingClient.MoveAsync(new()
                                        {
                                            Id = _userId,
                                            Position = _self.transform.position,
                                            Rotation = _self.transform.rotation
                                        });
                                    }, _shutdownCts.Token);
    }

    private async UniTaskVoid OnDestroy()
    {
        _shutdownCts.Cancel();

        if (streamingClient is not null)
        {
            await streamingClient.LeaveAsync();
            await streamingClient.DisposeAsync();
        }
        if (_channel is not null) { await _channel.ShutdownAsync(); }
    }

    public async UniTask InitializeClient(CancellationToken token)
    {
        _channel = GrpcChannelx.ForTarget(new("jane.jungle-gamedev.com", 5001, false));

        while (!_shutdownCts.IsCancellationRequested)
        {
            try
            {
                Debug.Log("Connecting to the server...");
                streamingClient = await StreamingHubClient.ConnectAsync<IGameHub, IGameHubReceiver>(_channel,
                                                                                                    this,
                                                                                                            cancellationToken: token);
                Debug.Log("Connection established!");
                break;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log($"Failed to connect to the server. Retrying after 5 seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: token);
        }

        _userId = Ulid.NewUlid();
        _self = await ConnectAsync(_channel, _roomId, _userId);
    }

    public async UniTask<GameObject> ConnectAsync(ChannelBase channel, Ulid roomId, Ulid userId)
    {
        _channel = channel;
        Player[] roomPlayers = await streamingClient.JoinAsync(roomId, userId, Vector3.zero, Quaternion.identity);

        foreach (Player player in roomPlayers)
        {
            if (player.Id == userId) { continue; }
            
            (this as IGameHubReceiver).OnJoin(player);
        }

        return players[userId];
    }

    public void OnJoin(Player request)
    {
        Debug.Log($"Player {request.Id} has joined the room.");

        GameObject playerInstance = Instantiate(playerPrefab, request.Position, request.Rotation);
        playerInstance.name = request.Id.ToString();
        players.TryAdd(request.Id, playerInstance);
    }

    public void OnLeave(Player request)
    {
        Debug.Log($"Player {request.Id} has left the room.");

        if (players.TryGetValue(request.Id, out GameObject cube))
        {
            players.Remove(request.Id);
            Destroy(cube);
        }
    }

    public void OnMove(MoveRequest request)
    {
        Debug.Log($"Mov Id:{request.Id}, x:{request.Position.x}, y:{request.Position.y}, z:{request.Position.z}\n" +
                         $"Rot x:{request.Rotation.x}, y:{request.Rotation.y}, z:{request.Rotation.z}, w:{request.Rotation.w}");

        if (players.TryGetValue(request.Id, out GameObject cube))
        {
            cube.transform.SetPositionAndRotation(request.Position, request.Rotation);
        }
    }
}
