using FluentAssertions;
using TaskManagementSystem.Dal.Models;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.IntegrationTests.Fakers;
using TaskManagementSystem.IntegrationTests.Fixtures;
using Xunit;

namespace TaskManagementSystem.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class UserRepositoryTests
{
    private readonly IUserRepository _repository;

    public UserRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.UserRepository;
    }

    [Fact]
    public async Task Add_User_Success()
    {
        // Arrange
        const int count = 5;

        var users = UserEntityV1Faker.Generate(count);
        
        // Act
        var results = await _repository.Add(users, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }
    
    [Fact]
    public async Task Get_SingleUser_Success()
    {
        // Arrange
        var users = UserEntityV1Faker.Generate();
        var userIds = await _repository.Add(users, default);
        var expectedUserId = userIds.First();
        var expectedUser = users.First()
            .WithId(expectedUserId);
        
        // Act
        var results = await _repository.Get(new UserGetModel()
        {
            UserIds = new[] { expectedUserId }
        }, default);
        
        // Asserts
        results.Should().HaveCount(1);
        var user = results.Single();

        user.Should().BeEquivalentTo(expectedUser);
    }
}