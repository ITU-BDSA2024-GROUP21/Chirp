using Microsoft.Data.Sqlite;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Chirp.Razor.Tests;

public class IntegrationTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public IntegrationTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async void testGetCheeps()
    {
        // Using in-memory SQLite db
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        await using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        var repository = new NootRepository(context, userManager: null);
        
        var cheeps = await repository.GetNoots(33);
        
        Assert.Empty(cheeps);
        Assert.NotNull(cheeps);
        
    }
}