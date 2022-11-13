using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{
    public interface ITypedConnectionMultiplexerClientFactory<TClient>
    {
        TClient CreateClient(IConnectionMultiplexer connectionMultiplexerClient);
    }
}
