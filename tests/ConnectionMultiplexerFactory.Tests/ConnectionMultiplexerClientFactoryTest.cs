using Microsoft.Extensions.DependencyInjection;
using ConnectionMultiplexerFactory.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{
    public class ConnectionMultiplexerClientFactoryTest
    {
        public ConnectionMultiplexerClientFactoryTest() { }

        [Fact]
        public void Factory_NamedClient_GetClient()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient("client_name").ConfigureConnect(x => mockedConnection);

            var services = serviceCollection.BuildServiceProvider();

            var client = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient("client_name");

            Assert.Equal(client, mockedConnection);
        }
        [Fact]
        public void Factory_NamedClients_CaseSensitive()
        {
            var mockedConnection_1 = new Mock<IConnectionMultiplexer>().Object;
            var mockedConnection_2 = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddConnectionMultiplexerClient("client_name").ConfigureConnect(x => mockedConnection_1);
            serviceCollection.AddConnectionMultiplexerClient("Client_name").ConfigureConnect(x => mockedConnection_2);

            var services = serviceCollection.BuildServiceProvider();

            var client1 = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient("client_name");
            var client2 = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient("Client_name");

            Assert.Equal(client1, mockedConnection_1);
            Assert.Equal(client2, mockedConnection_2);
        }
        [Fact]
        public void Factory_NamedClient_ConfigureClient()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();
            ConfigurationOptions? expectedConfiguration = null;

            short connectCounter = 0;
            serviceCollection
                .AddConnectionMultiplexerClient("client_name")
                .ConfigureClient(x => x.ClientName = "some_name")
                .ConfigureClient(x => x.ConnectTimeout = 1985)
                .ConfigureConnect(x =>
                {
                    connectCounter++;
                    expectedConfiguration = x;
                    return mockedConnection;
                });

            var services = serviceCollection.BuildServiceProvider();

            services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient("client_name");
            
            Assert.Equal(1, connectCounter);
            Assert.Equal("some_name", expectedConfiguration!.ClientName);
            Assert.Equal(1985, expectedConfiguration!.ConnectTimeout);
        }
        [Fact]
        public void Factory_NamedClient_SameNameSameInstance()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();

            short i = 0;
            serviceCollection
                .AddConnectionMultiplexerClient("client_name")
                .ConfigureClient(x => i++)
                .ConfigureConnect(x =>
                {
                    return mockedConnection;
                });

            var services = serviceCollection.BuildServiceProvider();

            var client1 = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient("client_name");
            var client2 = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient("client_name");

            Assert.Equal(1, i);
            Assert.Equal(client1, client2);
        }
        [Fact]
        public void Factory_NamedClient_NoSetup()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient("client_name");
            var services = serviceCollection.BuildServiceProvider();

            Assert.ThrowsAny<Exception>(() => services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient("client_name"));
        }
        [Fact]
        public void Factory_NamedClient_HorseWithNoName()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient("client_name");
            var services = serviceCollection.BuildServiceProvider();

            Assert.Throws<ArgumentNullException>(() => services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient(null!));
        }
        [Fact]
        public void Factory_NamedClient_DefaultClient()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();

            ConfigurationOptions? expectedConfiguration = null;

            serviceCollection
                .AddConnectionMultiplexerClient()
                .ConfigureClient(c => c.ClientName = "a_beutiful_client")
                .ConfigureConnect(c =>
                {
                    expectedConfiguration = c;
                    return mockedConnection;
                });

            var services = serviceCollection.BuildServiceProvider();

            var connectionMultiplexerClient = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient();

            Assert.Equal(mockedConnection, connectionMultiplexerClient);
            Assert.Equal("a_beutiful_client", expectedConfiguration!.ClientName);
        }
        [Fact]
        public void Factory_NamedClient_DefaultClientSameInstance()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddConnectionMultiplexerClient()
                .ConfigureConnect(c => mockedConnection);

            var services = serviceCollection.BuildServiceProvider();

            var connectionMultiplexerClient1 = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient();
            var connectionMultiplexerClient2 = services.GetRequiredService<IConnectionMultiplexerClientFactory>().CreateClient();

            Assert.Equal(connectionMultiplexerClient1, connectionMultiplexerClient2);
        }
    }
}