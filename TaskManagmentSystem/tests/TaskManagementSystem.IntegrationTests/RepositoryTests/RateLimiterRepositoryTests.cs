using FluentAssertions;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.IntegrationTests.Creators;
using TaskManagementSystem.IntegrationTests.Fixtures;
using Xunit;

namespace TaskManagementSystem.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class RateLimiterRepositoryTests
{
    private readonly IRateLimiterRepository _repository;

    public RateLimiterRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.RateLimiterRepository;
    }

    [Fact]
    public async Task GetRequestsNumber_SingleRequest_Success()
    {
        // Arrange
        var ip = Create.RandomUserIp();
        long expectedRequestsNumber = 1;

        // Act
        var actualRequestsNumber = await _repository.GetRequestsNumber(ip, default);
        
        // Asserts
        actualRequestsNumber.Should().Be(expectedRequestsNumber);
    }
}