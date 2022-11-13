using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ConnectionMultiplexerFactory.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace ConnectionMultiplexerFactory
{
    public class ConnectionMultiplexerClientBuilderTest
    {
        [Fact]
        public void Builder_ConfigureConfigurationBuilder_NoConfigRaiseArgumentNullException()
        {
            var builder = new ServiceCollection().AddConnectionMultiplexerClient("client_name");
            Assert.Throws<ArgumentNullException>(() => builder.ConfigureConfigurationBuilder(null!));
        }
        [Fact]
        public void Builder_WithConnectionString_BuildConfigOptions()
        {
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddConnectionMultiplexerClient("client_name").WithConnectionString("localhost:6576");

            var services = serviceCollection.BuildServiceProvider();
            var options = services.GetRequiredService<IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions>>().Get("client_name");

            Assert.Equal("localhost:6576", options.ConfigurationBuilder.Invoke().ToString());
        }
        [Fact]
        public void Builder_ConfigureConfigurationBuilder_CheckInvoke()
        {
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddConnectionMultiplexerClient("client_name");
            bool invokeCheck = false;
            builder.ConfigureConfigurationBuilder(() =>
            {
                invokeCheck = true;
                return new ConfigurationOptions();
            });

            var services = serviceCollection.BuildServiceProvider();
            var options = services.GetRequiredService<IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions>>().Get("client_name");

            options.ConfigurationBuilder.Invoke();

            Assert.True(invokeCheck);
        }
        [Fact]
        public void Builder_ConfigureClient_NoConfigRaiseArgumentNullException()
        {
            var builder = new ServiceCollection().AddConnectionMultiplexerClient("client_name");
            Assert.Throws<ArgumentNullException>(() => builder.ConfigureClient(null!));
        }
        [Fact]
        public void Builder_ConfigureClient_CheckInvoke()
        {
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddConnectionMultiplexerClient("client_name");
            short counter = 0;
            builder.ConfigureClient(c => counter++);
            builder.ConfigureClient(c => counter++);

            var services = serviceCollection.BuildServiceProvider();
            var options = services.GetRequiredService<IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions>>().Get("client_name");
            for (short i = 0; i < options.ConfigureClientActions.Count; i++)
            {
                options.ConfigureClientActions[i].Invoke(new ConfigurationOptions());
            }

            Assert.Equal(2, counter);
        }
        [Fact]
        public void Builder_ConfigureConnect_NoConfigRaiseArgumentNullException()
        {
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddConnectionMultiplexerClient("client_name");

            Assert.Throws<ArgumentNullException>(() => builder.ConfigureConnect(null!));
        }
        [Fact]
        public void Builder_ConfigureConnect_CheckInvoke()
        {
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddConnectionMultiplexerClient("client_name");
            bool checkInvoke = false;
            builder.ConfigureConnect(c =>
            {
                checkInvoke = true;
                return null!;
            });

            var services = serviceCollection.BuildServiceProvider();
            var options = services.GetRequiredService<IOptionsMonitor<ConnectionMultiplexerClientFactoryOptions>>().Get("client_name");

            options.Connect.Invoke(new ConfigurationOptions());

            Assert.True(checkInvoke);
        }
    }
}