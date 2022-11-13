using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory.DependencyInjection
{
    public static class ConnectionMultiplexerClientBuilderExtensions
    {
        public static IConnectionMultiplexerClientBuilder ConfigureConnect(this IConnectionMultiplexerClientBuilder builder, Func<ConfigurationOptions, IConnectionMultiplexer> connect)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(connect);

            builder.Services.Configure<ConnectionMultiplexerClientFactoryOptions>(builder.Name, options => options.Connect = connect);

            return builder;
        }
        public static IConnectionMultiplexerClientBuilder ConfigureClient(this IConnectionMultiplexerClientBuilder builder, Action<ConfigurationOptions> configureClient)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configureClient);

            builder.Services.Configure<ConnectionMultiplexerClientFactoryOptions>(builder.Name, options => options.ConfigureClientActions.Add(configureClient));

            return builder;
        }
        public static IConnectionMultiplexerClientBuilder WithConnectionString(this IConnectionMultiplexerClientBuilder builder, string connectionString)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(connectionString);

            builder.Services.Configure<ConnectionMultiplexerClientFactoryOptions>(builder.Name, options => options.ConfigurationBuilder = () => ConfigurationOptions.Parse(connectionString));

            return builder;
        }
        public static IConnectionMultiplexerClientBuilder ConfigureConfigurationBuilder(this IConnectionMultiplexerClientBuilder builder, Func<ConfigurationOptions> configureBuilder)
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configureBuilder);

            builder.Services.Configure<ConnectionMultiplexerClientFactoryOptions>(builder.Name, options => options.ConfigurationBuilder = configureBuilder);

            return builder;
        }
        internal static IConnectionMultiplexerClientBuilder AddTypedClient<TClient>(this IConnectionMultiplexerClientBuilder builder) where TClient : class
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.AddTransient(provider =>
            {
                var connectionMultiplexerClientFactory = provider.GetRequiredService<IConnectionMultiplexerClientFactory>();
                var connectionMultiplexerClient = connectionMultiplexerClientFactory.CreateClient(builder.Name);

                ITypedConnectionMultiplexerClientFactory<TClient> typedClientFactory = provider.GetRequiredService<ITypedConnectionMultiplexerClientFactory<TClient>>();
                return typedClientFactory.CreateClient(connectionMultiplexerClient);
            });

            return builder;
        }
    }
}
