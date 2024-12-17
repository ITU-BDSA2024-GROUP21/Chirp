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
    public required Process _serverProcess;
    public required IBrowser _browser;
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
    // This is testing of our whole program, and checks that everything is as we expect and that it contains the expected functionality
    public async Task End2EndTest1()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("username").ClickAsync();
        //CHANGE NUMBER HERE
        await Page.GetByPlaceholder("username").FillAsync("Helene");
        await Page.GetByPlaceholder("name@example.com").ClearAsync();
        //CHANGE NUMBER HERE
        await Page.GetByPlaceholder("name@example.com").FillAsync("helene@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj1!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj1!");
        // Note det er som om den ikke registrere vi kommer videre når vi klikker på linket register

        var _content = await Page.ContentAsync();
        Console.WriteLine(_content);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        

        //checking that we are now direkte to our public timeline
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public timeline" })).ToBeVisibleAsync();

        //Checking That Helene is logged in and can log out
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Logout Helene" })).ToBeVisibleAsync();
        //Checking that the Mytimeline button is visible
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();
        //Checking that the noot box is visible
        //CHANGE NUMBER HERE
        await Expect(Page.GetByText("Noot noot? Helene? Share")).ToBeVisibleAsync();
        await Page.Locator("#Text").ClickAsync();
        string uniqueMessage = $"hej med Dig - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

        await Page.Locator("#Text").FillAsync(uniqueMessage);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();



        //Checking that the Noot is visible on my timeline
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).ToBeVisibleAsync();

        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })
            .GetByRole(AriaRole.Button)).ToBeVisibleAsync();

        await Page.Locator("li").Filter(new() { HasText = uniqueMessage })
            .GetByRole(AriaRole.Button).ClickAsync();
       
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).Not.ToBeVisibleAsync();
        
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "About Me" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" }).ClickAsync();

    }

    // This is also a test of the whole program, where we test that it contains everything we expect
    [Test]
    public async Task End2EndTest2()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByPlaceholder("username").ClickAsync();
        //CHANGE NUMBER HERE
        await Page.GetByPlaceholder("username").FillAsync("Alan100");
        await Page.GetByPlaceholder("name@example.com").ClearAsync();
        //CHANGE NUMBER HERE
        await Page.GetByPlaceholder("name@example.com").FillAsync("Alan100@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj1!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj1!");

        var _content = await Page.ContentAsync();
        Console.WriteLine(_content);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        
        //checking that we are now direkte to our public timeline
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" })).ToBeVisibleAsync();
        
        //Checking That Alan is logged in and can log out
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Logout Alan100" })).ToBeVisibleAsync();

        //Checking that the noot box is visible
        await Expect(Page.GetByText("Noot noot? Alan100? Share")).ToBeVisibleAsync();

        await Page.Locator("#Text").ClickAsync();
        string uniqueMessage = $"hej med Dig - {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

        await Page.Locator("#Text").FillAsync(uniqueMessage);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        

        //Checking that the Noot is visible on public timeline
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();

        
        //Checking that the Noot is visible on my timeline
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).ToBeVisibleAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })
            .GetByRole(AriaRole.Button)).ToBeVisibleAsync();

        await Page.Locator("li").Filter(new() { HasText = uniqueMessage })
            .GetByRole(AriaRole.Button).ClickAsync();
       
        await Expect(Page.Locator("li").Filter(new() { HasText = uniqueMessage })).Not.ToBeVisibleAsync();
        
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "About Me" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" }).ClickAsync();

    }


}