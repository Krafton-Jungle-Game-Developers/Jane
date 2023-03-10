using System.IO;
using Grpc.Core;
using MagicOnion.Unity;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using UnityEngine;
using MagicOnion;
using System.Collections.Generic;
using System;

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
            Application.targetFrameRate = 144;
            System.Random random = new System.Random();
            List<string> names = new List<string>() { "Arrow", "Bjergson", "Canyon", "Doublelift", "Elyoya", "Faker", "GorillA", "Hai", "Ignar", "Jankos", "Karsa", "Labrov", "Mithy",
                                                      "Nemesis", "Oner", "Perkz", "Quas", "River", "Sneaky", "Tomo", "Uzi", "Vicla", "WildTurtle", "Xmithie", "YamatoCanon", "Zven"};
            string randomName = names[random.Next(names.Count)];
            string uniqueID = UserInfo.UniqueId.ToString();
            UserInfo.UserId = randomName + "#" + uniqueID.Substring(uniqueID.Length - 4);
        }
    }
}
