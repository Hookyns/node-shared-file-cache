using Xunit;

namespace RJDev.MemoryMappedCache.Tests;

public class ClientTests : IClassFixture<SocketServerFixture>
{
    [Fact]
    public void Test1()
    {
        using var client = Module.Module.CreateClient(9876);

        client.GetFile("index.ts").Then((content) =>
        {
            Assert.False(string.IsNullOrWhiteSpace(content.ToString()));
        }, err =>
        {
            Assert.True(false, err.Message);
        });
    }
}