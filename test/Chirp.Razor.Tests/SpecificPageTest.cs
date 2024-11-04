using System.Net.Http.Headers;
using AspNet.Security.OAuth.GitHub;

namespace Chirp.Razor.Tests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class SpecificPageTest : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    // Using a CustomWebApplicationFactory in order to overload ConfigureWebHost to replace default dbcontext with
    // our own seeded data
    public SpecificPageTest(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true,
            BaseAddress = new Uri("https://localhost:5773")
            
        });
        var clientId = Environment.GetEnvironmentVariable("CLIENT_ID") ?? "dummy-client-id";
        var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET") ?? "dummy-client-secret";
    }
    

    [Theory]
    [InlineData("?page=2")]
    public async Task GetCheepsSpecificPage(string page)
    {
        var response = await _client.GetAsync($"/{page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        response.Dispose();
        
        Assert.Contains("The student had drawn the body of it was I?", content);
        Assert.DoesNotContain("Starbuck now is what we hear the worst.", content);
    }

    [Theory]
    [InlineData("?page=6")]
    public async Task GetCheepsAuthorSpecificPage(string page)
    {
        var response = await _client.GetAsync($"/{page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        response.Dispose();
        
        Assert.Contains("Wendell Ballan", content);
        Assert.Contains("Johnnie Calixto", content);
        Assert.DoesNotContain("Helge", content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", "?page=3")]
    public async Task GetCheepsAuthorSpecificPage1(string author, string page)
    {
        var response = await _client.GetAsync($"/{author}/{page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        response.Dispose();

        Assert.Contains("No small number of days and such evidence.", content);
        Assert.Contains($"{author}'s Timeline", content);
        Assert.DoesNotContain("The young hunter''s dark face grew tense with emotion and anticipation.", content);
    }
    
    [Theory]
    [InlineData("Quintin Sitts", "?page=2")]
    public async Task GetCheepsAuthorSpecificPage2(string author, string page)
    {
        var response = await _client.GetAsync($"/{author}/{page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        response.Dispose();

        Assert.Contains("Colonel Sebastian Moran, who shot one of them described as dimly lighted?", content);
        Assert.Contains($"{author}'s Timeline", content);
        Assert.DoesNotContain("If I go, but Holmes caught up the side of mankind devilish dark at that.", content);
    }


}
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove any existing DbContexts
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChirpDBContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register ChirpDBContext as a Singleton in order to provide the same access for all tests
            services.AddDbContext<ChirpDBContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            }, ServiceLifetime.Singleton);
            
            // Create database similarly to in Program.cs
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            // Ensure fresh database and seed for each test run
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            DbInitializer.SeedDatabase(db);

            // Validate seeding
            var testAuthor = db.Authors.FirstOrDefault();
            if (testAuthor == null)
            {
                throw new Exception("Database seeding failed, no authors found");
            }
        });
    }
}
