using System.IO;
using Grpc.Core;
using MagicOnion.Unity;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using UnityEngine;
using MagicOnion;

namespace Assets.Scripts
{
    class InitialSettings
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterResolvers()
        {
            MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;
            MagicOnionMemoryPackFormatterProvider.RegisterFormatters();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeInitialize()
        {
            SslCredentials credentials =
                new(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "server.crt")));
            
            GrpcChannelProviderHost.Initialize(new DefaultGrpcChannelProvider(new GrpcCCoreChannelOptions(
                channelOptions: new[]
                {
                    // send keepalive ping every 5 second, default is 2 hours
                    new ChannelOption("grpc.keepalive_time_ms", 5000),
                    // keepalive ping time out after 5 seconds, default is 20 seconds
                    new ChannelOption("grpc.keepalive_timeout_ms", 5 * 1000)
                },
                channelCredentials: credentials
            )));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Application.targetFrameRate = 60;
        }
    }
}
