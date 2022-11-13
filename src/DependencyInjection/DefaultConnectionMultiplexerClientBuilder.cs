using Microsoft.Extensions.DependencyInjection;

namespace ConnectionMultiplexerFactory.DependencyInjection
{
    internal sealed class DefaultConnectionMultiplexerClientBuilder : IConnectionMultiplexerClientBuilder
    {
        public DefaultConnectionMultiplexerClientBuilder(IServiceCollection services, string name)
        {
            Services = services;
            Name = name;
        }
        public string Name { get; }
        public IServiceCollection Services { get; }
    }
}
