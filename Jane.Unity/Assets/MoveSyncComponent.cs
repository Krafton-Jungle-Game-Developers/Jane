using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using Jane.Unity.ServerShared;
using System;

public class MoveSyncComponent : MonoBehaviour
{
    private async UniTaskVoid Start()
    {
        await InitializeClientAsync();
    }

    private async UniTask InitializeClientAsync()
    {
        // Initialize the Hub
        _channel = GrpcChannelx.ForTarget(new("jane.jungle-gamedev.com", 5001, false));

        while (!_shutdownCts.IsCancellationRequested)
        {
            try
            {
                Debug.Log($"Connecting to the server...");
                _streamingClient = await StreamingHubClient.ConnectAsync<IChatHub, IChatHubReceiver>(_channel, this, cancellationToken: _shutdownCts.Token).AsUniTask();
                RegisterDisconnectEvent(_streamingClient).Forget();
                Debug.Log($"Connection is established.");
                break;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            Debug.Log($"Failed to connect to the server. Retry after 5 seconds...");
            await UniTask.Delay(5 * 1000);
        }

        _client = MagicOnionClient.Create<IChatService>(_channel);
    }
}
