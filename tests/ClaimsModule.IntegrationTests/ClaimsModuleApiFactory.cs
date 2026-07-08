using ClaimsModule.Application.Interfaces;
using ClaimsModule.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimsModule.IntegrationTests;

public sealed class ClaimsModuleApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string databaseName = $"ClaimsModuleIntegrationTests_{Guid.NewGuid():N}";

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await CreateDatabaseAsync();

        Client = CreateClient();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ClaimsModuleDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        Client.Dispose();
        Dispose();
        await DropDatabaseAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = BuildConnectionString(databaseName)
            };

            configuration.AddInMemoryCollection(settings);
        });
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IReserveGlPostingJobQueue, NoOpReserveGlPostingJobQueue>();
        });
    }

    private async Task CreateDatabaseAsync()
    {
        await using var connection = new SqlConnection(BuildConnectionString("master"));
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE [{databaseName}]";
        await command.ExecuteNonQueryAsync();
    }

    private async Task DropDatabaseAsync()
    {
        await using var connection = new SqlConnection(BuildConnectionString("master"));
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            IF DB_ID(N'{databaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END
            """;

        await command.ExecuteNonQueryAsync();
    }

    private static string BuildConnectionString(string database)
    {
        return new SqlConnectionStringBuilder
        {
            DataSource = "localhost,1433",
            InitialCatalog = database,
            UserID = "sa",
            Password = "Local_dev_password_123!",
            TrustServerCertificate = true,
            Encrypt = true
        }.ConnectionString;
    }
}

public sealed class NoOpReserveGlPostingJobQueue : IReserveGlPostingJobQueue
{
    public void EnqueueReservePosting(Guid reserveId)
    {
    }
}
