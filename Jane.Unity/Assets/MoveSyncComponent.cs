using UnityEngine;
using Cysharp.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using Grpc.Core;
using Grpc.Core.Api;
using Jane.Unity.ServerShared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jane.Unity.ServerShared.Hubs;
using Jane.Unity.ServerShared.MemoryPackObjects;

public class MoveSyncComponent : MonoBehaviour, IMovementHubReceiver
{
    private Dictionary<Ulid, GameObject> players = new();
    private readonly CancellationTokenSource _shutdownCts = new();
    private ChannelBase _channel;
    private IMovementHub streamingClient;
    private Ulid _roomId = Ulid.MinValue;
    private Ulid _userId;

    private async UniTaskVoid Start()
    {
        await InitializeClient(_shutdownCts.Token);
    }

    private async UniTaskVoid OnDestroy()
    {
        _shutdownCts.Cancel();

        if (streamingClient is not null) { await streamingClient.DisposeAsync(); }
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
                streamingClient = await StreamingHubClient.ConnectAsync<IMovementHub, IMovementHubReceiver>(_channel,
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

        await ConnectAsync(_channel, _roomId, _userId);
    }

    public async UniTask<GameObject> ConnectAsync(ChannelBase channel, Ulid roomId, Ulid userId)
    {
        _channel = channel;
        streamingClient = await StreamingHubClient.ConnectAsync<IMovementHub, IMovementHubReceiver>(channel, this);
        Player[] roomPlayers = await streamingClient.JoinAsync(roomId, userId, Vector3.zero, Quaternion.identity);

        foreach (var player in roomPlayers)
        {
            if (player.Id == userId) { continue; }

            (this as IMovementHubReceiver).OnJoin(player);
        }

        return players[userId];
    }

    public async UniTask LeaveAsync()
    {
        await streamingClient.LeaveAsync();
    }

    public ValueTask MoveAsync(MoveRequest request)
    {
        return streamingClient.MoveAsync(request);
    }

    public void OnJoin(Player request)
    {
        Debug.Log($"Player {request.Id} has joined the room.");

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = request.Id.ToString();
        cube.transform.SetPositionAndRotation(request.Position, request.Rotation);
        players.TryAdd(request.Id, cube);
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
        Debug.Log($"Player {request.Id} Move - Pos x:{request.Position.x}, y:{request.Position.y}, z:{request.Position.z}" +
                  $"Rot x:{request.Rotation.x}, y:{request.Rotation.y}, z:{request.Rotation.z}, w:{request.Rotation.w}");

        if (players.TryGetValue(request.Id, out GameObject cube))
        {
            cube.transform.SetPositionAndRotation(request.Position, request.Rotation);
        }
    }
}
