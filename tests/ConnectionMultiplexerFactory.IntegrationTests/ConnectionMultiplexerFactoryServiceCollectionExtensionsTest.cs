using Microsoft.Extensions.DependencyInjection;
using ConnectionMultiplexerFactory.DependencyInjection;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory.IntegrationTests
{
    public class ConnectionMultiplexerFactoryServiceCollectionExtensionsTest : IClassFixture<Fixture>
    {
        private readonly Fixture _fixture;

        public ConnectionMultiplexerFactoryServiceCollectionExtensionsTest(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async void AddConnectionMultiplexerClient_SetValue_ExpectedSameValueOnGet()
        {
            // preparation
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient().ConfigureConfigurationBuilder(() => ConfigurationOptions.Parse($"localhost:{_fixture.RedisMappedPublicPort}")); ;
            var services = serviceCollection.BuildServiceProvider();

            var factory = services.GetRequiredService<IConnectionMultiplexerClientFactory>();
            var db = factory.CreateClient().GetDatabase();

            // act
            await db.StringSetAsync("a_fake_key", "a_fake_value");
            var data = await db.StringGetAsync("a_fake_key");

            // assert
            Assert.Equal("a_fake_value", data);
        }
        [Fact]
        public async void AddConnectionMultiplexerClientT_SetValue_ExpectedSameValueOnGet()
        {
            // preparation
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddConnectionMultiplexerClient<Fixture.TypedClient>()
                .WithConnectionString($"localhost:{_fixture.RedisMappedPublicPort}");
            var services = serviceCollection.BuildServiceProvider();

            var client = services.GetRequiredService<Fixture.TypedClient>();
            string aGuid = Guid.NewGuid().ToString();

            // act
            await client.SetKeyValuePair(aGuid, "a_fake_value");
            var data = await client.GetValue(aGuid);

            // assert
            Assert.Equal("a_fake_value", data);
        }
    }
}