using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{   
    public static class ConnectionMultiplexerFactoryExtensions
    {
        public static IConnectionMultiplexer CreateClient(this IConnectionMultiplexerClientFactory factory)
        {
            ArgumentNullException.ThrowIfNull(factory);

            return factory.CreateClient(Options.DefaultName);
        }
    }
}
