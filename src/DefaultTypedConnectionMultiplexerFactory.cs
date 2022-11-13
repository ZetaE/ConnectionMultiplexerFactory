using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{
    public class DefaultTypedConnectionMultiplexerClientFactory<TClient> : ITypedConnectionMultiplexerClientFactory<TClient>
    {
        private readonly Cache _cache;
        private readonly IServiceProvider _services;

        public DefaultTypedConnectionMultiplexerClientFactory(Cache cache, IServiceProvider services)
        {
            ArgumentNullException.ThrowIfNull(cache);

            _cache = cache;
            _services = services;
        }
        public TClient CreateClient(IConnectionMultiplexer connectionMultiplexerClient)
        {
            ArgumentNullException.ThrowIfNull(connectionMultiplexerClient);

            return (TClient)_cache.Activator(_services, new object[] { connectionMultiplexerClient });
        }
        // The Cache should be registered as a singleton, so it that it can
        // act as a cache for the Activator. This allows the outer class to be registered
        // as a transient, so that it doesn't close over the application root service provider.
        // see https://github.com/dotnet/runtime/blob/46871b8541fbc2d2f3ff27207597b3a38792a010/src/libraries/Microsoft.Extensions.Http/src/DefaultTypedHttpClientFactory.cs
        public sealed class Cache
        {
            private static readonly Func<ObjectFactory> _createActivator = () => ActivatorUtilities.CreateFactory(typeof(TClient), new Type[] { typeof(IConnectionMultiplexer), });

            private ObjectFactory? _activator;
            private bool _initialized;
            private object? _lock;

            public ObjectFactory Activator => LazyInitializer.EnsureInitialized(
                ref _activator,
                ref _initialized,
                ref _lock,
                _createActivator)!;
        }
    }
}
