using HorizonChat.Services;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace HorizonChat.Tests.Services;

public class UsernameServiceTests
{
    [Fact]
    public async Task SetUsernameAsync_ShouldStoreUsername()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        var username = "TestUser";

        // Act
        await service.SetUsernameAsync(username);

        // Assert
        jsRuntimeMock.Verify(
            x => x.InvokeAsync<object>(
                "localStorage.setItem",
                It.Is<object[]>(args => 
                    args.Length == 2 && 
                    args[0].ToString() == "horizonChat_username" && 
                    args[1].ToString() == username)),
            Times.Once);
    }

    [Fact]
    public async Task SetUsernameAsync_ShouldUpdateCurrentUsername()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        var username = "TestUser";

        // Act
        await service.SetUsernameAsync(username);

        // Assert
        Assert.Equal(username, service.CurrentUsername);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task SetUsernameAsync_ShouldThrowForEmptyUsername(string? username)
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await service.SetUsernameAsync(username!));
    }

    [Fact]
    public async Task GetUsernameAsync_ShouldReturnStoredUsername()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var expectedUsername = "TestUser";
        jsRuntimeMock.Setup(x => x.InvokeAsync<string?>(
            "localStorage.getItem",
            It.IsAny<object[]>()))
            .ReturnsAsync(expectedUsername);
        
        var service = new UsernameService(jsRuntimeMock.Object);

        // Act
        var result = await service.GetUsernameAsync();

        // Assert
        Assert.Equal(expectedUsername, result);
    }

    [Fact]
    public async Task GetUsernameAsync_ShouldReturnCachedUsername()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        var username = "TestUser";
        await service.SetUsernameAsync(username);

        // Act
        var result1 = await service.GetUsernameAsync();
        var result2 = await service.GetUsernameAsync();

        // Assert
        Assert.Equal(username, result1);
        Assert.Equal(username, result2);
        // Verify localStorage.getItem was never called (using cache)
        jsRuntimeMock.Verify(
            x => x.InvokeAsync<string?>(
                "localStorage.getItem",
                It.IsAny<object[]>()),
            Times.Never);
    }

    [Fact]
    public async Task ClearUsernameAsync_ShouldRemoveUsername()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        await service.SetUsernameAsync("TestUser");

        // Act
        await service.ClearUsernameAsync();

        // Assert
        jsRuntimeMock.Verify(
            x => x.InvokeAsync<object>(
                "localStorage.removeItem",
                It.Is<object[]>(args => 
                    args.Length == 1 && 
                    args[0].ToString() == "horizonChat_username")),
            Times.Once);
    }

    [Fact]
    public async Task ClearUsernameAsync_ShouldClearCurrentUsername()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        await service.SetUsernameAsync("TestUser");

        // Act
        await service.ClearUsernameAsync();

        // Assert
        Assert.Null(service.CurrentUsername);
    }

    [Fact]
    public async Task HasUsername_ShouldReturnTrueWhenUsernameExists()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        await service.SetUsernameAsync("TestUser");

        // Act
        var result = service.HasUsername();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasUsername_ShouldReturnFalseWhenUsernameIsNull()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);

        // Act
        var result = service.HasUsername();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task OnUsernameChanged_ShouldTriggerWhenUsernameSet()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        var eventTriggered = false;
        service.OnUsernameChanged += () => eventTriggered = true;

        // Act
        await service.SetUsernameAsync("TestUser");

        // Assert
        Assert.True(eventTriggered);
    }

    [Fact]
    public async Task OnUsernameChanged_ShouldTriggerWhenUsernameCleared()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        var service = new UsernameService(jsRuntimeMock.Object);
        await service.SetUsernameAsync("TestUser");
        
        var eventTriggered = false;
        service.OnUsernameChanged += () => eventTriggered = true;

        // Act
        await service.ClearUsernameAsync();

        // Assert
        Assert.True(eventTriggered);
    }

    [Fact]
    public async Task GetUsernameAsync_ShouldHandleJSException()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(x => x.InvokeAsync<string?>(
            "localStorage.getItem",
            It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("localStorage not available"));
        
        var service = new UsernameService(jsRuntimeMock.Object);

        // Act
        var result = await service.GetUsernameAsync();

        // Assert
        Assert.Null(result);
    }
}
