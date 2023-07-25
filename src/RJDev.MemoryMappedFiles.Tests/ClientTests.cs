using Xunit;

namespace RJDev.MemoryMappedFiles.Tests;

public class ClientTests : IClassFixture<SocketServerFixture>
{
    [Fact]
    public void Test1()
    {
        using var client = Module.Module.CreateClient(9876);

        client.GetFileAsync("index.ts").Then((content) =>
        {
            Assert.False(string.IsNullOrWhiteSpace(content.ToString()));
        }, err =>
        {
            Assert.True(false, err.Message);
        });
    }
}