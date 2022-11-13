using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory.Tests
{
    public class TypedConnectionMultiplexerClientFactoryTest
    {
        [Fact]
        public void Factory_CreateTypedClient_CheckInjection()
        {
            var mockedConnection = new Mock<IConnectionMultiplexer>().Object;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IFakeService, FakeService>();
            var services = serviceCollection.BuildServiceProvider();

            var factory = new DefaultTypedConnectionMultiplexerClientFactory<TypedTestClient>(
                new DefaultTypedConnectionMultiplexerClientFactory<TypedTestClient>.Cache(),
                services);

            var client = factory.CreateClient(mockedConnection);

            Assert.Equal(mockedConnection, client.ConnectionMultiplexer);
            Assert.Equal("fake service", client.FakeService.GetName());
        }
        [Fact]
        public void Factory_CreateTypedClient_NullConnectionMultiplexerRaiseArgumentNullException()
        {
            var factory = new DefaultTypedConnectionMultiplexerClientFactory<TypedTestClient>(
                new DefaultTypedConnectionMultiplexerClientFactory<TypedTestClient>.Cache(),
                new ServiceCollection().BuildServiceProvider());

            Assert.Throws<ArgumentNullException>(() => factory.CreateClient(null!));
        }
    }
}