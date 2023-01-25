using Grpc.Core;
using MagicOnion.Serialization;
using System;

namespace MagicOnion.Client
{
    public static partial class MagicOnionClient
    {
        static readonly IClientFilter[] emptyFilters = Array.Empty<IClientFilter>();

        public static T Create<T>(ChannelBase channel)
            where T : IService<T>
        {
            return Create<T>(channel.CreateCallInvoker(), MagicOnionSerializerProvider.Default, emptyFilters, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(ChannelBase channel, IClientFilter[] clientFilters)
            where T : IService<T>
        {
            return Create<T>(channel.CreateCallInvoker(), MagicOnionSerializerProvider.Default, clientFilters, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(ChannelBase channel, IMagicOnionSerializerProvider serializerProvider)
            where T : IService<T>
        {
            return Create<T>(channel.CreateCallInvoker(), serializerProvider, emptyFilters, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(CallInvoker invoker)
            where T : IService<T>
        {
            return Create<T>(invoker, MagicOnionSerializerProvider.Default, emptyFilters, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(CallInvoker invoker, IClientFilter[] clientFilters)
            where T : IService<T>
        {
            return Create<T>(invoker, MagicOnionSerializerProvider.Default, clientFilters, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(CallInvoker invoker, IMagicOnionSerializerProvider serializerProvider)
            where T : IService<T>
        {
            return Create<T>(invoker, serializerProvider, emptyFilters, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(CallInvoker invoker, IMagicOnionSerializerProvider serializerProvider, IClientFilter[] clientFilters)
            where T : IService<T>
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));

            var clientOptions = new MagicOnionClientOptions(invoker, default, default, clientFilters);
            return Create<T>(clientOptions, serializerProvider, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(CallInvoker invoker, IMagicOnionSerializerProvider serializerProvider, IClientFilter[] clientFilters, IMagicOnionClientFactoryProvider clientFactoryProvider)
            where T : IService<T>
        {
            if (invoker == null) throw new ArgumentNullException(nameof(invoker));

            var clientOptions = new MagicOnionClientOptions(invoker, default, default, clientFilters);
            return Create<T>(clientOptions, serializerProvider, clientFactoryProvider);
        }

        public static T Create<T>(MagicOnionClientOptions clientOptions, IMagicOnionSerializerProvider serializerProvider)
            where T : IService<T>
        {
            return Create<T>(clientOptions, serializerProvider, MagicOnionClientFactoryProvider.Default);
        }

        public static T Create<T>(MagicOnionClientOptions clientOptions, IMagicOnionSerializerProvider serializerProvider, IMagicOnionClientFactoryProvider clientFactoryProvider)
            where T : IService<T>
        {
            if (serializerProvider is null) throw new ArgumentNullException(nameof(serializerProvider));
            if (clientFactoryProvider is null) throw new ArgumentNullException(nameof(clientFactoryProvider));

            if (!clientFactoryProvider.TryGetFactory<T>(out var factory))
            {
                throw new NotSupportedException($"Unable to get client factory for service type '{typeof(T).FullName}'.");
            }

            return factory(clientOptions, serializerProvider);
        }
    }
}
