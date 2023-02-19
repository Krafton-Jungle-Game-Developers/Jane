using UnityEngine;
using Cysharp.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using Grpc.Core;
using Grpc.Core.Api;
using Jane.Unity.ServerShared;
using System;
using System.Threading;

public class MoveSyncComponent : MonoBehaviour
{
    private readonly CancellationTokenSource _shutdownCts = new();
    private ChannelBase _channel;
    private async UniTaskVoid Start()
    {
    }

    private async UniTask InitializeClientAsync()
    {
        // Initialize the Hub
        _channel = GrpcChannelx.ForTarget(new("jane.jungle-gamedev.com", 5001, false));

        while (!_shutdownCts.IsCancellationRequested)
        {
            
        }
        
    }
}
