using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Assert = Xunit.Assert;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Chirp.Infrastructure;


using Chirp.Razor.Tests.PlaywrightTests;
using Microsoft.Data.Sqlite;

[Parallelizable(ParallelScope.None)]
[TestFixture]
public class UnitTest1 : PageTest
{
    private Process _serverProcess;
    protected IBrowser _browser;
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        };
    }

    [SetUp]
    public async Task Setup()
    {
        _serverProcess = await ServerUtil.StartServer();
        Thread.Sleep(5000); // Increase this if needed

        _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions {Headless = true});
        
        
    }
    
    [TearDown]
    public async Task Cleanup()
    {

        _serverProcess.Kill(true);

        _serverProcess.Dispose();
        await _browser.DisposeAsync();

    }
    
    //NOTE: PROGRAM SHOULD BE RUNNING BEFORE RUNNING THE UI TEST
    
    
    //NootBoxIsVisibleWhenloggedIn is testing that a nootchat is visible
    //when a user is logged in.
    [Test]
    public async Task NootBoxIsVisibleWhenloggedIn()
    {
        // Go to our webpage's main side
        await Page.GotoAsync("https://localhost:5273/");
        
        // CLick on the log in button
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
    
        //Typing in the mail and password for logging in
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        
        //Click on the login Button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        // Interact with the Noot-chat box
        await Page.Locator("#Text").ClickAsync();
        
        //Checks that the Noot-chat box is exiting
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();

        // Logging out of the website
        await Page.GetByRole(AriaRole.Link, new() { Name = "Logout Marcus" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }

    
    //NootBoxIsNotVisibleWhenNotloggedIn is testing that a nootchat isn't visible
    //when a user isn't logged in
    [Test]
    public async Task NootBoxIsNotVisibleWhenNotloggedIn()
    {
        //Goes to the main page and checks that the Noot-chat box isn't visible
        await Page.GotoAsync("https://localhost:5273/");
        await Expect(Page.Locator("#Text")).Not.ToBeVisibleAsync();
        
    }

    [Test]
    public async Task NootChatBoxCharacterLimit()
    {
       
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("#Text").ClickAsync();

        String InputValue160 =
            "jfdhfæahfiowehfoiwahefæowhfhjfjksbedfowehfioewhfoewihfjkbgvbdfhvæjdafihowejfpiowejfpoewjfowepjfopewjfoepjfpwøajf'wpjofp'woejhfvoewhbgvkbjelanfiewoøhfw'ihfiøwoeq";
        
        await Page.Locator("#Text").FillAsync(InputValue160);
        int length = InputValue160.Length;
        
        Assert.Equal(160,length);
    }
    [Test]  
    public async Task NootChatBoxCharacterLimitMoreCharacters()
    {
       
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("#Text").ClickAsync();

        String InputValue161 =
            "jfdhfæahfiowehfoiwahefæowhfhjfj2ksbedfowehfioewhfoewihfjkbgvbdfhvæjdafihowejfpiowejfpoewjfowepjfopewjfoepjfpwøajf'wpjofp'woejhfvoewhbgvkbjelanfiewoøhfw'ihfiøwoeq";
        
        await Page.Locator("#Text").FillAsync(InputValue161);
        
        string actualValue = await Page.Locator("#Text").InputValueAsync();
        
        Assert.Equal(160,actualValue.Length);
    }

    [Test]
    public async Task SendingCheepsWorks()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        //Checks that the Noot-chat box is exiting
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();
        //Checks that the Noot-chat box is exiting
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();
        await Page.Locator("#Text").ClickAsync();
        //Makesure the test passes everytime by adding a unique identifier like timestamp
        string uniqueMessage = $"hej med dig!! - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        await Page.Locator("#Text").FillAsync(uniqueMessage);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        // Checks That the Noot is visible after posting
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).ToBeVisibleAsync();
        
    }
    
    [Test]
    public async Task CheckingThatWeHandleXSSAttacks()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        // Checks that the Noot-chat box is exiting
        await Expect(Page.Locator("cheepbox")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();
        // Checks that the Noot-chat box is exiting
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();
        await Page.Locator("#Text").ClickAsync();
        // Make sure the test passes everytime by adding a unique identifier like timestamp
        string uniqueMessage = $"Hello, I am feeling good!<script>alert('If you see this in a popup, you are in trouble!');</script>\n\n - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        await Page.Locator("#Text").FillAsync(uniqueMessage);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        // Checks That the Noot is visible after posting
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).ToBeVisibleAsync();
        
        
    }

    [Test]
    public async Task CheckingThatWeHandleSQLInjections()
    {
        await Page.GotoAsync("https://localhost:5273/");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Robert'); DROP TABLE Students;-- ");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("robert@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj1!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj1!");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        
        var _content = await Page.ContentAsync();
        Console.WriteLine(_content);
        
        await Expect(Page.GetByText("Username 'Robert'); DROP TABLE Students;-- ' is invalid, can only contain letters or digits." )).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task RegisterTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
    
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
    
        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Carla49");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("carla49@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj691!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj691!");
    
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        // await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" })).ToBeVisibleAsync();
        // await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        
    
    }
    
    [Test]
    public async Task DeleteTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        
        // Log in and navigate to users timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();
        
        // Make Noot and check for existence
        await Page.Locator("#Text").ClickAsync();
        string uniqueMessage = $"hej med dig!! - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        await Page.Locator("#Text").FillAsync(uniqueMessage);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync(); // Not sure why but it is necessary manually to navigate to personal timeline again
        var specificCheep = Page.Locator("ul#messagelist li").Filter(new() { HasText = uniqueMessage });
        await Expect(specificCheep).ToBeVisibleAsync();
        
        // Delete Noot and check that it no longer exists on users timeline
        await specificCheep.GetByAltText("Delete").ClickAsync();

        await Expect(specificCheep).Not.ToBeVisibleAsync();
    
    }
    
    [Test]
    public async Task FollowTest1()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" })).ToBeVisibleAsync();
        
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Follow logo")).ToBeVisibleAsync();
        
        await Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Follow logo").ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();
        
        // Conforming that the following photo isn't visible for brian anymor because we are
        // know following hum and the unfollow image is now visible
        await Expect(Page.Locator("li")
                .Filter(new() { HasText = "brian2 hej — 27.11.2024" })
                .Locator("img[alt='Follow logo']"))
            .Not.ToBeVisibleAsync();

        
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync(); 
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" })).ToBeVisibleAsync();

        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();
        
        var _content = await Page.ContentAsync();
        Console.WriteLine(_content);
        
    }
    
    [Test]
    public async Task UnFollowTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" })).ToBeVisibleAsync();
        
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync(); 
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" })).ToBeVisibleAsync();

        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();

        
        await Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Unfollow logo").ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" })).Not.ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public timeline" }).ClickAsync(); 
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Unfollow logo")).Not.ToBeVisibleAsync();

        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej — 27.11.2024" }).GetByAltText("Follow logo")).ToBeVisibleAsync();
        
        // Conforming that the following photo isn't visible for brian anymor because we are
        // know following hum and the unfollow image is now visible
        await Expect(Page.Locator("li")
                .Filter(new() { HasText = "brian2 hej — 27.11.2024" })
                .Locator("img[alt='Unfollow logo']"))
            .Not.ToBeVisibleAsync();

        
        var _content = await Page.ContentAsync();
        Console.WriteLine(_content);
        
    }

    [Test]
    public async Task NextPage()
    {
        await Page.GotoAsync("https://localhost:5273/");
        
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Next Page" }).ClickAsync();
        
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        string currentUrl = Page.Url;
        StringAssert.Contains("/?page=2", currentUrl);
    }
    
    [Test]
    public async Task PrevPage()
    {
        await Page.GotoAsync("https://localhost:5273/?page=4");
        
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Previous Page" }).ClickAsync();
        
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        string currentUrl = Page.Url;
        StringAssert.Contains("/?page=3", currentUrl);
    }

    [Test]
    public async Task NextPageUserTimeLine()
    {
        await Page.GotoAsync("https://localhost:5273/Jacqualine%20Gilcoine");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Next Page" }).ClickAsync();
        
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        string currentUrl = Page.Url;
        
        StringAssert.Contains("/Jacqualine%20Gilcoine/?page=2", currentUrl);
        
    }

    [Test]
    public async Task NoNextPageRegister()
    {
        await Page.GotoAsync("https://localhost:5273/Register");

        var nextPageButton = await Page.IsVisibleAsync("a:has-text('Next Page')");
        
        Assert.False(nextPageButton);

    }
    [Test]
    public async Task NoPreviousPage()
    {
        await Page.GotoAsync("https://localhost:5273/?page=1");

        var nextPageButton = await Page.IsVisibleAsync("a:has-text('Previous Page')");
        
        Assert.False(nextPageButton);

    }
}
