using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{
    public class ConnectionMultiplexerClientFactoryOptions
    {
        public Func<ConfigurationOptions> ConfigurationBuilder { get; set; } = () => new ConfigurationOptions();
        public IList<Action<ConfigurationOptions>> ConfigureClientActions { get; } = new List<Action<ConfigurationOptions>>();
        public Func<ConfigurationOptions, IConnectionMultiplexer> Connect { get; set; } = c => ConnectionMultiplexer.Connect(c);
    }
}