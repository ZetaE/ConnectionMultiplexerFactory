using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure an <see cref="IServiceCollection"/> for <see cref="IConnectionMultiplexerClientFactory"/>.
    /// </summary>
    public static class ConnectionMultiplexerFactoryServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all services needed by ConnectionMultiplexerFactory
        /// </summary>
        /// <param name="services"></param>
        /// <returns>Default ConnectionMultiplexer client builder</returns>
        public static IConnectionMultiplexerClientBuilder AddConnectionMultiplexerClient(this IServiceCollection services)
        {
            services.AddOptions();

            services.TryAddSingleton<DefaultConnectionMultiplexerClientFactory>();
            services.TryAddSingleton((Func<IServiceProvider, IConnectionMultiplexerClientFactory>)(serviceProvider => serviceProvider.GetRequiredService<ConnectionMultiplexerFactory.DefaultConnectionMultiplexerClientFactory>()));

            return new DefaultConnectionMultiplexerClientBuilder(services, Options.DefaultName);
        }
        public static IConnectionMultiplexerClientBuilder AddConnectionMultiplexerClient(this IServiceCollection services, string name)
        {
            ArgumentNullException.ThrowIfNull(name);

            _ = services.AddConnectionMultiplexerClient();

            return new DefaultConnectionMultiplexerClientBuilder(services, name);
        }
        public static IConnectionMultiplexerClientBuilder AddConnectionMultiplexerClient(this IServiceCollection services,
            string name,
            string connectionString)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(connectionString);

            _ = services.AddConnectionMultiplexerClient();

            var builder = new DefaultConnectionMultiplexerClientBuilder(services, name);
            builder.ConfigureConfigurationBuilder(() => ConfigurationOptions.Parse(connectionString));

            return builder;
        }
        public static IConnectionMultiplexerClientBuilder AddConnectionMultiplexerClient(this IServiceCollection services,
            string name,
            ConfigurationOptions configurationOptions)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(configurationOptions);

            _ = AddConnectionMultiplexerClient(services);

            var builder = new DefaultConnectionMultiplexerClientBuilder(services, name);
            builder.ConfigureConfigurationBuilder(() => configurationOptions);

            return builder;
        }
        public static IConnectionMultiplexerClientBuilder AddConnectionMultiplexerClient<TClient>(this IServiceCollection services) where TClient : class
        {
            if (typeof(TClient).FullName is not string name)
            {
                throw new InvalidOperationException("Connection multiplexer client invalid type");
            }

            _ = services.AddConnectionMultiplexerClient();
            _ = services.AddTypedActivator();

            var builder = new DefaultConnectionMultiplexerClientBuilder(services, name);
            builder.AddTypedClient<TClient>();

            return builder;
        }
        public static IServiceCollection AddConnectionMultiplexerClient<TClient>(this IServiceCollection services, string connectionString) where TClient : class
        {
            ArgumentNullException.ThrowIfNull(connectionString);

            var builder = services.AddConnectionMultiplexerClient<TClient>();
            builder.ConfigureConfigurationBuilder(() => ConfigurationOptions.Parse(connectionString));

            return services;
        }
        public static IServiceCollection AddConnectionMultiplexerClient<TClient>(this IServiceCollection services, ConfigurationOptions options) where TClient : class
        {
            ArgumentNullException.ThrowIfNull(options);

            var builder = services.AddConnectionMultiplexerClient<TClient>();
            builder.ConfigureConfigurationBuilder(() => options);

            return services;
        }
        internal static IServiceCollection AddTypedActivator(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Transient(typeof(ITypedConnectionMultiplexerClientFactory<>), typeof(DefaultTypedConnectionMultiplexerClientFactory<>)));
            services.TryAdd(ServiceDescriptor.Singleton(typeof(DefaultTypedConnectionMultiplexerClientFactory<>.Cache), typeof(DefaultTypedConnectionMultiplexerClientFactory<>.Cache)));

            return services;
        }
    }
}
