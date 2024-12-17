using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Assert = Xunit.Assert;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Chirp.Infrastructure;


using Chirp.Razor.Tests.PlaywrightTests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


[Parallelizable(ParallelScope.None)]
[TestFixture]
public class UnitTests : PageTest
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

    public static UserManager<ApplicationUser> GetUserManager(ChirpDBContext context)
    {
        var userStore = new UserStore<ApplicationUser>(context);
        var optionsManager = Options.Create(new IdentityOptions());
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var userValidators = new List<IUserValidator<ApplicationUser>> { new UserValidator<ApplicationUser>() };
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>>
            { new PasswordValidator<ApplicationUser>() };
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

    public async Task<IAuthorRepository> RepositorySetUp()
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
        return new AuthorRepository(context, userManager);
    }

    [SetUp]
    public async Task Setup()
    {
        _serverProcess = await ServerUtil.StartServer();
        Thread.Sleep(5000); // Increase this if needed

        _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    [TearDown]
    public async Task Cleanup()
    {

        _serverProcess.Kill(true);

        _serverProcess.Dispose();
        await _browser.DisposeAsync();

    }


    [Test]
    public async Task NavigationPublicTimelineButtonTest()
    {
        await Page.GotoAsync("https://localhost:5273/");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Public timeline" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task NavigationPublicTimelineButtonWorkTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public timeline" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new (){Name = "Public Timeline"})).ToBeVisibleAsync();
    }

    [Test]
    public async Task NavigationRegisterButtonTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        
        await Expect(Page.GetByRole(AriaRole.Link, new() {Name = "Register"})).ToBeVisibleAsync();
    }

    [Test]
    public async Task NavigationRegisterButtonWorkTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() {Name = "Register"}).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() {Name = "Register", Exact = true})).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task NavigationLoginButtonTest()
    {
        await Page.GotoAsync("https://localhost:5273/");

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task NavigationLoginButtonWorkTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() {Name = "Login"}).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() {Name = "Log in", Exact = true})).ToBeVisibleAsync();
    }

    [Test]
    public async Task RegisterPageTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() {Name = "Register", Exact = true})).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." })).ToBeVisibleAsync();

        await Expect(Page.GetByPlaceholder("username")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Username")).ToBeVisibleAsync();
        
        await Expect(Page.GetByPlaceholder("name@example.com")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Email")).ToBeVisibleAsync();
        
        await Expect(Page.GetByLabel("Password", new(){Exact = true})).ToBeVisibleAsync();
        await Expect(Page.GetByText("Password", new(){Exact = true})).ToBeVisibleAsync();
        
        await Expect(Page.GetByLabel("Confirm Password")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Confirm Password")).ToBeVisibleAsync();

        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Register" })).ToBeVisibleAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Use another service to register." }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" })).ToBeVisibleAsync();
        

    }

    [Test]
    public async Task LoginPageTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() {Name = "Login"}).ClickAsync();
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() {Name = "Log in", Exact = true})).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Use a local account to log in." })).ToBeVisibleAsync();
        
        await Expect(Page.GetByPlaceholder("name@example.com")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Email",new(){Exact = true})).ToBeVisibleAsync();
        
        await Expect(Page.GetByPlaceholder("password")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Password",new(){Exact = true})).ToBeVisibleAsync();

        await Expect(Page.GetByLabel("Remember me?")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Remember me?")).ToBeVisibleAsync();
        
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Log in" })).ToBeVisibleAsync();
        
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Forgot your password?" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register as a new user" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Resend email confirmation" })).ToBeVisibleAsync();
        
        
       await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Use another service to log in." }))
            .ToBeVisibleAsync();

        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "GitHub" })).ToBeVisibleAsync();
                

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
    [Test]
    public async Task BioOnUserWhenNotLoggedIn()
    {
        await Page.GotoAsync("https://localhost:5273/");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Birthe" }).ClickAsync();
        await Expect(Page.GetByText("Birthe's BIO")).ToBeVisibleAsync();
        await Expect(Page.GetByText("hejsa")).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Public timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "brian2" }).ClickAsync();
        await Expect(Page.GetByText("brian2's BIO")).ToBeVisibleAsync();
        await Expect(Page.GetByText("There are no Bio so far.")).ToBeVisibleAsync();
    }
}
    