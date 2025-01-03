using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace Chirp.Razor.Tests;
using Microsoft.Data.Sqlite;
using Xunit;

public class UnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public static UserManager<ApplicationUser> GetUserManager(ChirpDBContext context)
    {
        var userStore = new UserStore<ApplicationUser>(context);
        var optionsManager = Options.Create(new IdentityOptions());
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var userValidators = new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() };
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() };
        var lookupNormalizer = new UpperInvariantLookupNormalizer();
        var errorDescriber = new IdentityErrorDescriber();
        var services = new ServiceCollection();
        var logger = new Logger<UserManager<ApplicationUser>>(new LoggerFactory());

        var userManager = new UserManager<ApplicationUser>(
            userStore,
            optionsManager,
            passwordHasher,
            userValidators,
            passwordValidators,
            lookupNormalizer,
            errorDescriber,
            services.BuildServiceProvider(),
            logger
        );
        return userManager;
    }

    // This sets up a nootRepository
    public async Task<INootRepository> NootRepositorySetUp()
    {
        // This is to create an in-memory SQLite connection
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        // Then we create the ChirpDBContext instance with builder.Options
        var context = new ChirpDBContext(builder.Options);
        var userManager = GetUserManager(context);
    
        // Then we ensure that the database schema is created
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        // In the end we return the CheepRepository instance for testing
        return new NootRepository(context, userManager);
    }
    
    // This sets up for the repository for Author
    public async Task<IAuthorRepository> AuthorRepositorySetUp()
    {
        var repositoryFollow = FollowRepositorySetUp();
        // This is to create an in-memory SQLite connection
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        // Then we create the ChirpDBContext instance with builder.Options
        var context = new ChirpDBContext(builder.Options);
        var userManager = GetUserManager(context);
    
        // Then we ensure that the database schema is created
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        // In the end we return the CheepRepository instance for testing
        return new AuthorRepository(context, userManager);
    }
    
    // This sets up the repository for Follow
    public async Task<IFollowRepository> FollowRepositorySetUp()
    {
        // This is to create an in-memory SQLite connection
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        // Then we create the ChirpDBContext instance with builder.Options
        var context = new ChirpDBContext(builder.Options);
        var userManager = GetUserManager(context);
    
        // Then we ensure that the database schema is created
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        // In the end we return the CheepRepository instance for testing
        return new FollowRepository(context, userManager);
    }
    
    [Fact]
    // This test that the repository is set up but contains nothing yet
    public async Task CheepRepositoryTest()
    {
        var repository1 = await NootRepositorySetUp();
        
        var cheeps = await repository1.GetNoots(1);

        Assert.Empty(cheeps);
        Assert.NotNull(cheeps);
    }
    
    [Fact]
    // This test that we can retrieve the Author by email
    public async Task GetAuthorByEmailTest()
    {
        // Arrange
        var repository2 = await AuthorRepositorySetUp();

        var testAuthor = new Author
        {
            Name = "Gummi Tarzan",
            Email = "Gummi.Tarzan@gmail.com",
            Cheeps = null!
        };

        // Seed the in-memory database with the test author
        await repository2.ConvertAuthors(new AuthorDTO { Name = testAuthor.Name, Email = testAuthor.Email });

        // Act
        var result = await repository2.GetAuthorByEmail(testAuthor.Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testAuthor.Email, result.Email);
    }

    [Fact]
    // This test that we can retrieve the Author by name
    public async Task GetAuthorByNameTest()
    {
        var repository3 = await AuthorRepositorySetUp();

        var testAuthor1 = new Author
        {
            Name = "Scooby Doo",
            Email = "Scooby.Doo@gmail.com",
            Cheeps = null!
        };

        await repository3.ConvertAuthors(new AuthorDTO { Name = testAuthor1.Name, Email = testAuthor1.Email });

        var result = await repository3.GetAuthorByName(testAuthor1.Name);

        Assert.NotNull(result);
        Assert.Equal(testAuthor1.Name, result.Name);
    }

    [Fact]
    // This is testing that we create a new author correctly
    public async Task CreateNewAuthorTest()
    {
        
        var repository4 = await AuthorRepositorySetUp();

        var testAuthorDTO = new AuthorDTO
        {
            Name = "Percy Jackson",
            Email = "Percy.Jackson@gmail.com"
        };

        var result = await repository4.ConvertAuthors(testAuthorDTO);

        Assert.NotNull(result);
        Assert.Equal(testAuthorDTO.Name, result.Name);
        Assert.Equal(testAuthorDTO.Email, result.Email);
    }

    [Fact]
    // This is testing that we can create a noot correclty
    public async Task CreateNoot()
    {
        var repository5 = await NootRepositorySetUp();

        var testAuthor1 = new Author
        {
            Name = "Tom Holland",
            Email = "Tom.Holland@gmail.com",
            AuthorId = 6000,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };

        var testCheepDTO = new CheepDTO
        {
            Text = "This is a test",
            Author = testAuthor1.ToString()!,
            TimeStamp = "2023-07-21 10:30:45",
            CheepId = 2147483647,
            AuthorId = testAuthor1.AuthorId
        };
        var testAuthorDTO1 = new AuthorDTO
        {
            Name = "Tom Holland",
            Email = "Tom.Holland@gmail.com"
        };
        

        await repository5.ConvertNoots(testCheepDTO, testAuthorDTO1);

        var cheepsByAuthor = await repository5.GetNootsFromAuthor(testAuthor1.Name, 0);

        Assert.NotNull(testAuthor1);
        Assert.Equal(testCheepDTO.Text, cheepsByAuthor.First().Text);
    }

    [Fact]
    // This checks that we delete an author correctly from the database, and that it no longer exists
    public async Task DeleteAuthorTest()
    {
        var repository6 = await AuthorRepositorySetUp();

        var testAuthorDTO = new AuthorDTO
        {
            Name = "Percy_Jackson1",
            Email = "Percy.Jackson1@gmail.com"
        };
        
        var Author = await repository6.ConvertAuthors(testAuthorDTO);
        _testOutputHelper.WriteLine(Author.Email);
        Assert.Equal(await repository6.CheckAuthorExists(testAuthorDTO), Author);
        var email = Author.Email;
       
        
        await repository6.DeleteAuthorByEmail(email);
        
       Assert.Null(await repository6.CheckAuthorExists(testAuthorDTO));
    }

    [Fact]
    // This tests that we delete a noot correctly from the database, and that it no longer exists
    public async Task DeleteNootTest()
    {

        var repository5 = await NootRepositorySetUp();

        var testAuthor1 = new Author
        {
            Name = "Tom Holland",
            Email = "Tom.Holland@gmail.com",
            AuthorId = 6000,
        };
          var testCheepDTO = new CheepDTO
        {
            Text = "This is a test",
            Author = testAuthor1.ToString()!,
            TimeStamp = "2023-07-21 10:30:45",
            CheepId = 2147483647,
            AuthorId = testAuthor1.AuthorId
        };
        var testAuthorDTO1 = new AuthorDTO
        {
            Name = "Tom Holland",
            Email = "Tom.Holland@gmail.com"
        };
              await repository5.ConvertNoots(testCheepDTO, testAuthorDTO1);

        var cheepsByAuthor = await repository5.GetNootsFromAuthor(testAuthor1.Name, 0);

        Assert.NotNull(testAuthor1);
        Assert.Equal(testCheepDTO.Text, cheepsByAuthor.First().Text);
        
        await repository5.DeleteNoot(cheepsByAuthor.First().CheepId);
        
        Assert.Empty(await repository5.GetNootsFromAuthor(testAuthor1.Name, 0));
    }
      
    [Fact]
    // This test that it stored correctly when a user follows another user
    public async Task FollowTest()
    {
        var repository6 = await AuthorRepositorySetUp();
        var testAuthor1 = new Author
        {
            Name = "John10",
            Email = "John10@gmail.com",
            AuthorId = 6001,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };
        await repository6.AddAuthorToDB(testAuthor1);
        var testAuthor2 = new Author
        {
            Name = "John11",
            Email = "John11@gmail.com",
            AuthorId = 6002,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };
        await repository6.AddAuthorToDB(testAuthor2);
        await repository6.FollowAuthor(testAuthor1.AuthorId, testAuthor2.AuthorId);

        var follows = await repository6.GetFollowedAuthors(testAuthor1.AuthorId);
        Assert.Contains(testAuthor2.Name, follows);
        
    }
    [Fact]
    // This test that it stored correctly when a user wants to unfollows another user
    public async Task UnfollowTest()
    {
        var repository7 = await AuthorRepositorySetUp();
        var testAuthor1 = new Author
        {
            Name = "John10",
            Email = "John10@gmail.com",
            AuthorId = 6001,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };

        await repository7.AddAuthorToDB(testAuthor1);
        var testAuthor2 = new Author
        {
            Name = "John11",
            Email = "John11@gmail.com",
            AuthorId = 6002,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };
        await repository7.AddAuthorToDB(testAuthor2);
        await repository7.FollowAuthor(testAuthor1.AuthorId, testAuthor2.AuthorId);

        var follows = await repository7.GetFollowedAuthors(testAuthor1.AuthorId);
        Assert.Contains(testAuthor2.Name, follows);
        
        await repository7.Unfollow(testAuthor1.AuthorId, testAuthor2.AuthorId);
        var follows2 = await repository7.GetFollowedAuthors(testAuthor1.AuthorId);

        Assert.DoesNotContain(testAuthor2.Name, follows2);
        
    }
}
    
