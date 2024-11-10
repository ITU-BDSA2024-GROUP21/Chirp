using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;


[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class UnitTest1 : PageTest
{
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
    public async Task NootBoxChatCharectarLimit()
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
    
}