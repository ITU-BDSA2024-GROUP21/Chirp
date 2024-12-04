using Xunit.Abstractions;

namespace Chirp.Razor.Tests;
using System.Threading.Tasks;
using Xunit;

public class APITest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    public APITest(WebApplicationFactory<Program> fixture, ITestOutputHelper testOutputHelper)
    {

        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
        _client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });
    }
    
    [Fact]
    public async Task CanSeePublicTimeline()
    {
        string page = "1";
        
        while(true)
        {
            var response = await _client.GetAsync($"/?page={page}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            
            if (content.Contains("Starbuck now is what we hear the worst."))
            {
                break;
            } else if (content.Contains("There are no cheeps so far."))
            {
                break;
            }

            int _page = int.Parse(page);
            _page += 1;
            page = _page.ToString();
        }
        var _response = await _client.GetAsync($"/?page={page}");
        _response.EnsureSuccessStatusCode();
        var _content = await _response.Content.ReadAsStringAsync();
        
        Assert.Contains("Public Timeline", _content);
        Assert.Contains("Jacqualine Gilcoine", _content);
        Assert.Contains("Starbuck now is what we hear the worst.", _content);
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

        Assert.Contains("Nooter", content);
        Assert.Contains($"{author}'s Timeline", content);
    }


    [Theory]
    [InlineData("Adrian")]
    public async Task CanSeeHTTPBody(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Nooter", content);
        Assert.Contains("Hej, velkommen til kurset", content);

    }
    [Theory]
    [InlineData("Helge")]
    public async Task CanSeeHTTPBody2(string author1)
    {
        var response = await _client.GetAsync($"/{author1}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Nooter", content);
        Assert.Contains("Hello, BDSA students!", content);
    }

    [Theory]
    [InlineData("Quintin Sitts")]
    public async Task LastPage(string author)
    {
        int page = 1;
        bool textFound = false;
        bool nextPageExists = true;

        while (nextPageExists)
        {
            var response = await _client.GetAsync($"/{author}/?page={page}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            if (content.Contains(
                    "He had, as you perceive, was made that suggestion to me that no wood is in reality his wife."))
            {
                textFound = true;
            }

            nextPageExists = content.Contains("Next Page");

            if (textFound && !nextPageExists)
            {
                break;
            }

            page++;
        }

        Assert.True(textFound);
        Assert.False(nextPageExists);
    }
}