namespace Chirp.Razor.Tests;
using Chirp.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

public class UnitTest
{
    public async Task<ICheepRepository> RepositorySetUp()
    {
        // This is to create an in-memory SQLite connection
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        // Then we create the ChirpDBContext instance with builder.Options
        var context = new ChirpDBContext(builder.Options);
    
        // Then we ensure that the database schema is created
        await context.Database.EnsureCreatedAsync();

        // In the end we return the CheepRepository instance for testing
        return new CheepRepository(context);
    }
    [Fact]
    public async Task CheepRepositoryTest()
    {
        var repository1 = await RepositorySetUp();
        
        var cheeps = await repository1.GetCheeps(1);

        Assert.Empty(cheeps);
        Assert.NotNull(cheeps);
    }
    
    [Fact]
    public async Task GetAuthorByEmailTest()
    {
        // Arrange
        var repository2 = await RepositorySetUp();

        var testAuthor = new Author
        {
            Name = "Gummi Tarzan",
            Email = "Gummi.Tarzan@gmail.com"
        };

        // Seed the in-memory database with the test author
        await repository2.CreateAuthors(new AuthorDTO { Name = testAuthor.Name, Email = testAuthor.Email });

        // Act
        var result = await repository2.GetAuthorByEmail(testAuthor.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testAuthor.Email, result.Email);
        //Assert.Equal(testAuthor.Name, result.Name);
    }

    [Fact]
    public async Task GetAuthorByNameTest()
    {
        var repository3 = await RepositorySetUp();

        var testAuthor1 = new Author
        {
            Name = "Scooby Doo",
            Email = "Scooby.Doo@gmail.com"
        };

        await repository3.CreateAuthors(new AuthorDTO { Name = testAuthor1.Name, Email = testAuthor1.Email });

        var result = await repository3.GetAuthorByName(testAuthor1.Name);

        Assert.NotNull(result);
        Assert.Equal(testAuthor1.Name, result.Name);
        //Assert.Equal(testAuthor1.Email, result.Email);
    }

    [Fact]
    public async Task CreateNewAuthorTest()
    {
        var repository4 = await RepositorySetUp();

        var testAuthorDTO = new AuthorDTO
        {
            Name = "Percy Jackson",
            Email = "Percy.Jackson@gmail.com"
        };

        var result = await repository4.CreateAuthors(testAuthorDTO);

        Assert.NotNull(result);
        Assert.Equal(testAuthorDTO.Name, result.Name);
        Assert.Equal(testAuthorDTO.Email, result.Email);
    }

    [Fact]
    public async Task CreateCheep()
    {
        var repository5 = await RepositorySetUp();

        var testAuthorDTO1 = new AuthorDTO
        {
            Name = "Tom Holland",
            Email = "Tom.Holland@gmail.com"
        };

        var testCheepDTO = new CheepDTO
        {
            Text = "This is a test",
            Author = testAuthorDTO1.ToString(),
            TimeStamp = "2023-07-21 10:30:45"
        };

        await repository5.CreateCheeps(testCheepDTO, testAuthorDTO1);

        var createdAuthor = await repository5.GetAuthorByName(testAuthorDTO1.Name);
        var cheepsByAuthor = await repository5.GetCheepsFromAuthor(createdAuthor.Name, 0);

        Assert.NotNull(createdAuthor);
        Assert.Equal(testCheepDTO.Text, cheepsByAuthor.First().Text);
    }
    
}