using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory.IntegrationTests
{
    public class Fixture : IAsyncLifetime
    {
        private const ushort _redisContainerPort = 6379;
        private readonly RedisTestcontainer _aRedisTestcontainer;
        internal ushort RedisMappedPublicPort { get => _aRedisTestcontainer.GetMappedPublicPort(_redisContainerPort); }
        
        public Fixture()
        {
            _aRedisTestcontainer = new TestcontainersBuilder<RedisTestcontainer>()
                .WithImage("redis")
                .WithPortBinding(_redisContainerPort, true)
                .Build();
        }

        public async Task DisposeAsync()
        {
            await _aRedisTestcontainer.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            await _aRedisTestcontainer.StartAsync();
        }

        public class TypedClient
        {
            private readonly IConnectionMultiplexer _connectionMultiplexer;

            public TypedClient(IConnectionMultiplexer connectionMultiplexer)
            {
                _connectionMultiplexer = connectionMultiplexer;
            }
            public async Task SetKeyValuePair(string key, string value)
            {
                await _connectionMultiplexer.GetDatabase().StringSetAsync(key, value);
            }
            public async Task<string?> GetValue(string key)
            {
                return await _connectionMultiplexer.GetDatabase().StringGetAsync(key);
            }
        }
    }
}
