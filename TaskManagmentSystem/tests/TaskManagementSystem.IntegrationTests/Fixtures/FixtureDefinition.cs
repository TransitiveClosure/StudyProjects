using Xunit;

namespace TaskManagementSystem.IntegrationTests.Fixtures
{
    [CollectionDefinition(nameof(TestFixture))]
    public class FixtureDefinition : ICollectionFixture<TestFixture>
    {
    }
}
