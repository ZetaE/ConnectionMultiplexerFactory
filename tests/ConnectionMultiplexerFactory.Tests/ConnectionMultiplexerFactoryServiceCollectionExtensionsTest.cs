using Microsoft.Extensions.DependencyInjection;
using ConnectionMultiplexerFactory.DependencyInjection;
using Moq;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

namespace ConnectionMultiplexerFactory.Tests
{
    public class ConnectionMultiplexerFactoryServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterFactory_DefaultFactory()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient();
            var services = serviceCollection.BuildServiceProvider();

            var factory = services.GetRequiredService<IConnectionMultiplexerClientFactory>();

            Assert.NotNull(factory);
            Assert.IsType<DefaultConnectionMultiplexerClientFactory>(factory);
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClient_DefaultFactory()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient("fake_client");
            var services = serviceCollection.BuildServiceProvider();

            var factory = services.GetRequiredService<IConnectionMultiplexerClientFactory>();

            Assert.NotNull(factory);
            Assert.IsType<DefaultConnectionMultiplexerClientFactory>(factory);
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClient_DefaultClientBuilder()
        {
            var builder = new ServiceCollection().AddConnectionMultiplexerClient("fake_client");

            Assert.IsAssignableFrom<IConnectionMultiplexerClientBuilder>(builder);
            Assert.Equal("fake_client", builder.Name);
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNullNamedClient_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddConnectionMultiplexerClient(null!));
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientConnectionString_DefaultClientBuilder()
        {
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddConnectionMultiplexerClient("fake_client", "fake_connection_string");

            Assert.IsAssignableFrom<IConnectionMultiplexerClientBuilder>(builder);
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientConnectionString_CheckConfiguration()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient("fake_client", "fake_connection_string");
            var services = serviceCollection.BuildServiceProvider();

            var options = services.GetRequiredService<IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions>>().Get("fake_client");
            var configuration = options.ConfigurationBuilder.Invoke();

            Assert.Equal("fake_connection_string", configuration.ToString());
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientWithNullConnectionString_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddConnectionMultiplexerClient("fake_client", (string)null!));
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientWithConnectionStringAndNullName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddConnectionMultiplexerClient(null!, "fake_conn_string"));
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientConfigurationOptions_DefaultClientBuilder()
        {
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddConnectionMultiplexerClient("fake_client", new ConfigurationOptions());

            Assert.IsAssignableFrom<IConnectionMultiplexerClientBuilder>(builder);
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientConfigurationOptions_CheckConfiguration()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();
            var configuration = new ConfigurationOptions();
            serviceCollection.AddConnectionMultiplexerClient("fake_client", configuration);
            var services = serviceCollection.BuildServiceProvider();

            var options = services.GetRequiredService<IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions>>().Get("fake_client");
            var configurationBuilderResult = options.ConfigurationBuilder.Invoke();

            Assert.Equal(configuration, configurationBuilderResult);
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientConfigurationOptionsWithNullName_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddConnectionMultiplexerClient(null!, new ConfigurationOptions()));
        }
        [Fact]
        public void AddConnectionMultiplexerClient_RegisterNamedClientConfigurationOptionsWithNullConfig_ThrowsArgNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ServiceCollection().AddConnectionMultiplexerClient("fake_client", (ConfigurationOptions)null!));
        }
        [Fact]
        public void AddConnectionMultiplexerClientT_RegisterTypedClient_DefaultTypedClientFactory()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient<FakeType>();
            var services = serviceCollection.BuildServiceProvider();

            var factory = services.GetRequiredService<ITypedConnectionMultiplexerClientFactory<FakeType>>();
            var cache = services.GetRequiredService<DefaultTypedConnectionMultiplexerClientFactory<FakeType>.Cache>();

            Assert.IsType<DefaultTypedConnectionMultiplexerClientFactory<FakeType>>(factory);
            Assert.IsType<DefaultTypedConnectionMultiplexerClientFactory<FakeType>.Cache>(cache);
        }
        [Fact]
        public void AddConnectionMultiplexerClientT_RegisterTypedClient_CheckInjection()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;
            var fakeService = new FakeService();

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddConnectionMultiplexerClient<TypedTestClient>()
                .ConfigureConnect(x => mockedConnection);
            serviceCollection.AddTransient<IFakeService>(s => fakeService);
            var services = serviceCollection.BuildServiceProvider();

            var typedClient = services.GetRequiredService<TypedTestClient>();

            Assert.Equal(mockedConnection, typedClient.ConnectionMultiplexer);
            Assert.Equal(fakeService, typedClient.FakeService);
        }
    }
}   