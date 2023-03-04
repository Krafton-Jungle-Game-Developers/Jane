using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Unity;
using UnityEngine;

public class GrpcChannelManager : MonoBehaviour
{
    private GrpcChannelx? gRPCChannel;
    public ChannelBase GRPCChannel => gRPCChannel;
    public static readonly string Host = "jane.jungle-gamedev.com";
    public static readonly int Port = 5001;
    public static readonly bool IsInsecure = false;

    private GrpcChannelTarget ChannelTarget = new(Host, Port, IsInsecure);

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(this);

        InitializeChannel();
    }

    private void InitializeChannel()
    {
        gRPCChannel = GrpcChannelx.ForTarget(ChannelTarget);
    }

    private async UniTaskVoid OnDestroy()
    {
        if (gRPCChannel is null) { return; }

        await gRPCChannel.DisposeAsync();
    }
}
