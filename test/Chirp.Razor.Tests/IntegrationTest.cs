using Microsoft.Data.Sqlite;
using Xunit.Abstractions;

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

        var repository = new CheepRepository(context);
        
        var cheeps = await repository.GetCheeps(33);
        
        Assert.Empty(cheeps);
        Assert.NotNull(cheeps);
        
        /*
        CheepDTO expectedCheepDTO = new CheepDTO
        {
            Author = "TestAuthor",
            Text = "Test",
            TimeStamp = "2023-08-01 13:14:38"
        } ;

        _testOutputHelper.WriteLine("__________________________________________________________");
        _testOutputHelper.WriteLine(cheeps.Count.ToString());

        // Unpacks first element in returned list of Cheeps for testing
        CheepDTO resultCheepDTO = new CheepDTO
        {
            Author = cheeps[0].Author,
            Text = cheeps[0].Text,
            TimeStamp = cheeps[0].TimeStamp
        } ;
        */
                
        // Assert.Equal(expectedCheepDTO, resultCheepDTO);
    }
}