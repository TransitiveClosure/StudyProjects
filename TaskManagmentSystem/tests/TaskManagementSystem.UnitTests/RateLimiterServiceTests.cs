using Moq;
using TaskManagementSystem.Bll.Exceptions;
using TaskManagementSystem.Bll.Services;
using TaskManagementSystem.Bll.Services.Interfaces;
using TaskManagementSystem.Dal.Repositories.Interfaces;

namespace TaskManagementSystem.UnitTests;

public class RateLimiterServiceTests
{
    private readonly Mock<IRateLimiterRepository> _rateLimiterRepositoryFake = new(MockBehavior.Strict);
    private readonly IRateLimiterService _rateLimiterService;

    public RateLimiterServiceTests()
    {
        _rateLimiterService = new RateLimiterService(_rateLimiterRepositoryFake.Object);
    }

    [Fact]
    public async Task ThrowIfTooManyRequests_SingleRequest_Success()
    {
        // Arrange
        var ip = "0.0.0.0";
        long requestsNumber = 1;
        _rateLimiterRepositoryFake.Setup(f => f.GetRequestsNumber(ip, default))
            .Returns(Task.FromResult(requestsNumber));

        // Act
        await _rateLimiterService.ThrowIfTooManyRequests(ip, default);

        // Asserts
        _rateLimiterRepositoryFake.Verify(f => f.GetRequestsNumber(ip, default), Times.Once);
    }

    [Fact]
    public async Task ThrowIfTooManyRequests_OverLimitRequest_DoNotCatchInvalidOperationException()
    {
        // Arrange
        var ip = "0.0.0.0";
        _rateLimiterRepositoryFake.Setup(f => f.GetRequestsNumber(ip, default))
            .Throws(new TooManyRequestsException(ip));
        
        // Act & Asserts
        await Assert.ThrowsAsync<TooManyRequestsException>(async () =>
            await _rateLimiterService.ThrowIfTooManyRequests(ip, default));
        _rateLimiterRepositoryFake.Verify(f => f.GetRequestsNumber(ip, default), Times.Once);
    }
}