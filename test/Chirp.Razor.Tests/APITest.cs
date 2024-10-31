namespace Chirp.Razor.Tests;
using System.Threading.Tasks;

public class APITest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public APITest(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });
    }
    
    [Fact]
    public async Task CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
        Assert.Contains("Public Timeline", content);
        Assert.Contains("Jacqualine Gilcoine", content);
        Assert.Contains("Starbuck now is what we hear the worst.", content);
    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    [InlineData("Roger Histand")]
    public async Task CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Twitter", content);
        Assert.Contains($"{author}'s Timeline", content);
    }


    [Theory]
    [InlineData("Adrian")]
    public async Task CanSeeHTTPBody(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Twitter", content);
        Assert.Contains("Hej, velkommen til kurset", content);

    }
    [Theory]
    [InlineData("Helge")]
    public async Task CanSeeHTTPBody2(string author1)
    {
        var response = await _client.GetAsync($"/{author1}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Twitter", content);
        Assert.Contains("Hello, BDSA students!", content);
    }
}