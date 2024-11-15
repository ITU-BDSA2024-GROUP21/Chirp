using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Assert = Xunit.Assert;
using NUnit.Framework;


using Chirp.Razor.Tests.PlaywrightTests;


[Parallelizable(ParallelScope.None)]
[TestFixture]
public class UIEnd2EndTest : PageTest
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

    [Test]
    public async Task End2EndTest1()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Alan");
        await Page.GetByPlaceholder("name@example.com").ClearAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("Alan@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj1!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj1!");
        // Note det er som om den ikke registrere vi kommer videre når vi klikker på linket register
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        //await Page.GotoAsync("https://localhost:5273/Identity/Account/RegisterConfirmation?email=alan@mail.dk&returnUrl=%2F");
        //await Expect(Page).ToHaveURLAsync("https://localhost:5273/Identity/Account/RegisterConfirmation?email=alan@mail.dk&returnUrl=%2F");
        
        //Checking that we can confirm our email
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Click here to confirm your" })).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        

        //checking that we are now direkte to our public timeline
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        

        //Checkign That Alan is logged in and can log out
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Logout Alan" })).ToBeVisibleAsync();
        //Checking that the my timeline button is visible
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" })).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();
        //Checking that the noot box is visible
        await Expect(Page.GetByText("Noot noot? Alan? Share")).ToBeVisibleAsync();
        
        await Page.Locator("#Text").ClickAsync();
        await Page.Locator("#Text").FillAsync("Hej med Dig");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();
        
        string uniqueMessage = $"hej med Dig - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        
        //Checking that the Noot is visible on my timeline
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Logout Alan" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

    }
    
}