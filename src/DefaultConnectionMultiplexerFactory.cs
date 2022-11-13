using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{
    /// <summary>
    /// Default implementation of <see cref="IConnectionMultiplexer"/>
    /// </summary>
    public class DefaultConnectionMultiplexerClientFactory : IConnectionMultiplexerClientFactory
    {
        private readonly ConcurrentDictionary<string, IConnectionMultiplexer?> _activeConnections = new(StringComparer.Ordinal);
        private readonly IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions> _options;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsMonitor"></param>
        public DefaultConnectionMultiplexerClientFactory(IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions> optionsMonitor)
        {
            _options = optionsMonitor;
        }
        private static IConnectionMultiplexer BuildClient(ConnectionMultiplexerClientFactoryOptions options)
        {
            var configuration = options.ConfigurationBuilder.Invoke();

            for (int i = 0; i < options.ConfigureClientActions.Count; i++)
            {
                options.ConfigureClientActions[i].Invoke(configuration);
            }

            return options.Connect.Invoke(configuration);
        }
        /// <summary>
        /// Create ConnectionMultiplexer client based on named configuration
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IConnectionMultiplexer CreateClient(string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            _activeConnections.TryGetValue(name, out IConnectionMultiplexer? registeredConnectionMultiplexer);

            if (registeredConnectionMultiplexer is not null)
            {
                return registeredConnectionMultiplexer;
            }

            if (_options.Get(name) is not ConnectionMultiplexerClientFactoryOptions options)
            {
                throw new InvalidOperationException($"Client {name} not configured");
            }

            var connectionMultiplexer = BuildClient(options);

            _activeConnections.AddOrUpdate(name, connectionMultiplexer, (k, v) => connectionMultiplexer);

            return connectionMultiplexer;
        }
    }
}
