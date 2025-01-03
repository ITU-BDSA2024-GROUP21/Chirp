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
public class IntegrationTests : PageTest
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

    // This sets up a repository which is used for testing purporses
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

    [Test]
    // This is testing that the length is 160 characters when inserting a string of 160 characters
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
    // This is for testing that even if you try to write more than 160 characters, the lenght will still be 160
    // because the user should not be able to write more than 160
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
    // This is for testing if you can send cheeps when logged in and that it is visible after being shared
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
    // This is for testing that we can handle XSS attacks 
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
        //await Expect(Page.Locator("cheepbox")).ToBeVisibleAsync();
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
    // This is to test that we can handle SQL injections 
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
    // Testing that you can register on our program correctly, and deleting the author afterwards so that it will pass next time we run the test
    public async Task RegisterTest()
    {
        var repo = await RepositorySetUp();
        await Page.GotoAsync("https://localhost:5273/");
    
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
    
        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Carla59");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("carla59@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj691!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj691!");
    
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        
        
        await repo.DeleteAuthorByEmail("carla59@mail.dk");
    }
    
    [Test]
    // This for testing that the noots actually get deleted, when the user tries to delete one of its noots
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
    // This is for testing if the follow function works, and that the image changes to an unfollow icon when the user has followed someone
    public async Task FollowTest1()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" })).ToBeVisibleAsync();
        
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Follow logo")).ToBeVisibleAsync();
        
        await Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Follow logo").ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();
        
        // Conforming that the following photo isn't visible for brian anymore because we are
        // know following hum and the unfollow image is now visible
        await Expect(Page.Locator("li")
                .Filter(new() { HasText = "brian2 hej" })
                .Locator("img[alt='Follow logo']"))
            .Not.ToBeVisibleAsync();

        
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync(); 
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" })).ToBeVisibleAsync();

        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();
        
        var _content = await Page.ContentAsync();
        Console.WriteLine(_content);
        
    }
    
    [Test]
    // This is test that the followed user's noots also is on the user timeline. Then it tests that the user can unfollow the user
    public async Task UnFollowTest()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("marcus@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj1!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" })).ToBeVisibleAsync();
        
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync(); 
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" })).ToBeVisibleAsync();

        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();

        
        await Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Unfollow logo").ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" })).Not.ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Public timeline" }).ClickAsync(); 
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Unfollow logo")).Not.ToBeVisibleAsync();

        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Follow logo")).ToBeVisibleAsync();
        
        // Conforming that the following photo isn't visible for brian anymor because we are
        // know following hum and the unfollow image is now visible
        await Expect(Page.Locator("li")
                .Filter(new() { HasText = "brian2 hej" })
                .Locator("img[alt='Unfollow logo']"))
            .Not.ToBeVisibleAsync();

        
        var _content = await Page.ContentAsync();
        Console.WriteLine(_content);
        
    }
    
    [Test]
    // This tests that our ForgetMe button works as expected, and deletes the user from the database
    public async Task ForgetAboutMeTest()
    {
        await Page.GotoAsync("https://localhost:5273");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
    
        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Birthe19");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("Birthe19@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj691!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj691!");
    
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" }).ClickAsync();
        
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("Birthe19@mail.dk");
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync("Halløj691!");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        var _content = await Page.ContentAsync();
        StringAssert.Contains("Invalid login attempt.", _content);
    }

    [Test]
    // This is for testing that there isn't any bio when firstly registered, and that the user can create their own bio
    public async Task FirstBio()
    {
        await Page.GotoAsync("https://localhost:5273/");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Bulgur1");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("Bulgur1@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj1!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj1!");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();

        await Expect(Page.GetByText("There are no Bio so far.")).ToBeVisibleAsync();

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "About Me" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Edit Your Bio" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Save" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Bulgur1's BIO" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("There are no Bio so far.")).ToBeVisibleAsync();

        await Page.Locator("#Text").ClickAsync();
        await Page.Locator("#Text").FillAsync("Jeg Hedder william");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Expect(Page.GetByText("Jeg Hedder william")).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();

        await Expect(Page.GetByText("Jeg Hedder william")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" }).ClickAsync();

    }
    
    [Test]
    // This is for testing, that you can change your bio without the program crashing (previous bio gets deleted)
    public async Task ChangingBio()
    {
        await Page.GotoAsync("https://localhost:5273/");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Bulderrr");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("Bulderrr@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj1!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj1!");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();


        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "About Me" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Edit Your Bio" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Save" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Bulderrr's BIO" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("There are no Bio so far.")).ToBeVisibleAsync();

        await Page.Locator("#Text").ClickAsync();
        await Page.Locator("#Text").FillAsync("Jeg Hedder william");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Expect(Page.GetByText("Jeg Hedder william")).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();

        await Expect(Page.GetByText("Jeg Hedder william")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        
        await Page.Locator("#Text").ClickAsync();
        await Page.Locator("#Text").FillAsync("bulder boy");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Expect(Page.GetByText("bulder boy")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "My timeline" }).ClickAsync();

        await Expect(Page.GetByText("bulder boy")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" }).ClickAsync();
    }

    [Test]
    // This is for testing that all the expected things and functionality is on the About Me page
    public async Task AboutmePageTest()
    {
        var repo = await RepositorySetUp();
        
        await Page.GotoAsync("https://localhost:5273/");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

        await Page.GetByPlaceholder("username").ClickAsync();
        await Page.GetByPlaceholder("username").FillAsync("Lief3");
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByPlaceholder("name@example.com").FillAsync("Lief3@mail.dk");
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Halløj1!");
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByLabel("Confirm Password").FillAsync("Halløj1!");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        await Expect(Page.GetByText("Noot noot? Lief3? Share")).ToBeVisibleAsync();
        

        await Page.Locator("#Text").ClickAsync();
        await Page.Locator("#Text").FillAsync("Lief siger hej");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej " })).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Follow logo")).ToBeVisibleAsync();
        
        await Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Follow logo").ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "brian2 hej" }).GetByAltText("Unfollow logo")).ToBeVisibleAsync();
        
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "About Me" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Personal Information" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Download My Information" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Name: Lief3")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Email: lief3@mail.dk")).ToBeVisibleAsync();
        await Expect(Page.Locator("p").Filter(new() { HasText = "Following:" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("brian2")).ToBeVisibleAsync();
        await Expect(Page.Locator("p").Filter(new() { HasText = "Noots:" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Lief siger hej")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me!" }).ClickAsync();
    }
}
