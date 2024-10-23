namespace Chirp.Razor.Tests;
using Microsoft.AspNetCore.Mvc.Testing;

public class SpecificPageTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;
    
    public SpecificPageTest(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true,
            HandleCookies = true
        });
    }

    [Theory]
    [InlineData("?page=2")]
    public async Task GetCheepsSpecificPage(string page)
    {
        var response = await _client.GetAsync($"/{page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        
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

        Assert.Contains("Colonel Sebastian Moran, who shot one of them described as dimly lighted?", content);
        Assert.Contains($"{author}'s Timeline", content);
        Assert.DoesNotContain("If I go, but Holmes caught up the side of mankind devilish dark at that.", content);
    }


}