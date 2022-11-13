using StackExchange.Redis;

namespace ConnectionMultiplexerFactory.Tests
{
    internal class FakeType
    {
        
    }
    internal class TypedTestClient
    {
        public readonly IConnectionMultiplexer ConnectionMultiplexer;
        public readonly IFakeService FakeService;
        public TypedTestClient(IFakeService fakeService, IConnectionMultiplexer connectionMultiplexer)
        {
            ConnectionMultiplexer = connectionMultiplexer;
            FakeService = fakeService;
        }
    }
    internal interface IFakeService
    {
        public string GetName();
    }
    internal class FakeService : IFakeService
    {
        public string GetName() => "fake service";
    }
}
