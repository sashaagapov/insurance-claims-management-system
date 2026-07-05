namespace ClaimsModule.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<ClaimsModuleApiFactory>
{
    public const string Name = "Claims module integration tests";
}
