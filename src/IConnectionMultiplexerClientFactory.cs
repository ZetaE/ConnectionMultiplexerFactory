using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{
    public interface IConnectionMultiplexerClientFactory
    {
        IConnectionMultiplexer CreateClient(string name);
    }
}