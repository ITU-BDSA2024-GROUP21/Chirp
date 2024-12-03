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

    public async Task<ICheepRepository> RepositorySetUp()
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
        return new CheepRepository(context, userManager);
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
        //Assert.Equal(testAuthor.Name, result.Name);
    }

    [Fact]
    public async Task GetAuthorByNameTest()
    {
        var repository3 = await RepositorySetUp();

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

        var result = await repository4.ConvertAuthors(testAuthorDTO);

        Assert.NotNull(result);
        Assert.Equal(testAuthorDTO.Name, result.Name);
        Assert.Equal(testAuthorDTO.Email, result.Email);
    }

    [Fact]
    public async Task CreateCheep()
    {
        var repository5 = await RepositorySetUp();

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
        

        await repository5.ConvertCheeps(testCheepDTO, testAuthorDTO1);

        var cheepsByAuthor = await repository5.GetCheepsFromAuthor(testAuthor1.Name, 0);

        Assert.NotNull(testAuthor1);
        Assert.Equal(testCheepDTO.Text, cheepsByAuthor.First().Text);
    }

    [Fact]
    public async Task DeleteAuthorTest()
    {
        var repository6 = await RepositorySetUp();

        var testAuthorDTO = new AuthorDTO
        {
            Name = "Percy_Jackson1",
            Email = "Percy.Jackson1@gmail.com"
        };
        
        var Author = await repository6.ConvertAuthors(testAuthorDTO);
        _testOutputHelper.WriteLine(Author.Email);
        Assert.Equal(await repository6.CheckAuthorExists(testAuthorDTO), Author);
        var email = Author.Email;
       
        
        await repository6.DeleteAuthorAndCheepsByEmail(email);
        
       Assert.Null(await repository6.CheckAuthorExists(testAuthorDTO));
    }

    [Fact]
    public async Task DeleteCheepTest()
    {

        var repository5 = await RepositorySetUp();

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
              await repository5.ConvertCheeps(testCheepDTO, testAuthorDTO1);

        var cheepsByAuthor = await repository5.GetCheepsFromAuthor(testAuthor1.Name, 0);

        Assert.NotNull(testAuthor1);
        Assert.Equal(testCheepDTO.Text, cheepsByAuthor.First().Text);
        
        await repository5.DeleteCheep(cheepsByAuthor.First().CheepId);
        
        Assert.Empty(await repository5.GetCheepsFromAuthor(testAuthor1.Name, 0));
    }
      
    [Fact]
    public async Task FollowTest()
    {
        var repository6 = await RepositorySetUp();
        var testAuthor1 = new Author
        {
            Name = "John10",
            Email = "John10@gmail.com",
            AuthorId = 6001,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };
        await repository6.AddAuthorAsync(testAuthor1);
        var testAuthor2 = new Author
        {
            Name = "John11",
            Email = "John11@gmail.com",
            AuthorId = 6002,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };
        await repository6.AddAuthorAsync(testAuthor2);
        await repository6.FollowAuthor(testAuthor1.AuthorId, testAuthor2.AuthorId);

        var follows = await repository6.GetFollowedAuthorsAsync(testAuthor1.AuthorId);
        Assert.Contains(testAuthor2.Name, follows);
        
    }
    [Fact]
    public async Task UnfollowTest()
    {
        var repository7 = await RepositorySetUp();
        var testAuthor1 = new Author
        {
            Name = "John10",
            Email = "John10@gmail.com",
            AuthorId = 6001,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };

        await repository7.AddAuthorAsync(testAuthor1);
        var testAuthor2 = new Author
        {
            Name = "John11",
            Email = "John11@gmail.com",
            AuthorId = 6002,
            Cheeps = null!,
            Followers = null!,
            Following = null!
        };
        await repository7.AddAuthorAsync(testAuthor2);
        await repository7.FollowAuthor(testAuthor1.AuthorId, testAuthor2.AuthorId);

        var follows = await repository7.GetFollowedAuthorsAsync(testAuthor1.AuthorId);
        Assert.Contains(testAuthor2.Name, follows);
        
        await repository7.Unfollow(testAuthor1.AuthorId, testAuthor2.AuthorId);
        var follows2 = await repository7.GetFollowedAuthorsAsync(testAuthor1.AuthorId);

        Assert.DoesNotContain(testAuthor2.Name, follows2);
        
    }
}
    
