using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using Microsoft.Build.Framework;
namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INooterService _nooterService;

    [BindProperty]
    public required string? Email { get; set; }
    public required List<String> FollowersList { get; set; }
    public required string Followers { get; set; }
    public required List<Cheep> CheepsList { get; set; }
    public  required List<string> CheepsListString;
    public required string Cheeps { get; set; }
    public BioDTO? Bio { get; set; }

    public AboutMeModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, INooterService nooterService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _nooterService = nooterService;
        CheepsListString = new List<string>();
        
    }

    [BindProperty]
    public BioBoxModel? BioInput { get; set; }
    
    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }
        
        //Loading The users email address
        Author author = await _nooterService.GetAuthorByName(User.Identity?.Name!);
        Email = author.Email;

        if (await _nooterService.AuthorHasBio(author.Name))
        {
            Bio = await _nooterService.GetBio(User.Identity?.Name!);
        }

        int authorid = author.AuthorId;
        
        //loading who our user is following
        FollowersList = await _nooterService.GetFollowedAuthors(authorid);
        Followers = string.Join( ", ", FollowersList.ToArray() );
        
        //loading all the cheeps
        CheepsList = await _nooterService.GetNootsWithoutPage(author.Name);

        foreach (Cheep cheep in CheepsList)
        {
            string cheepo = cheep.Text;
            Console.WriteLine("Hello " + cheepo);
            CheepsListString.Add(cheepo);
            
        }
        
        Cheeps = string.Join(", ", CheepsListString.ToArray() );

        return Page();
    }
    // This is when a user wants to download the information stored about them
    public async Task<IActionResult> OnPostDownload()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound("User not found.");
        }
        Author author1 = await _nooterService.GetAuthorByName(User.Identity?.Name!);
        if (await _nooterService.AuthorHasBio(author1.Name))
        {
            Bio = await _nooterService.GetBio(User.Identity?.Name!);
        }

        var personalData = new Dictionary<string, object>();

        
        var author = await _nooterService.GetAuthorByName(User.Identity?.Name!);
        if (author != null!)
        {
            personalData.Add("Name", author.Name);
            personalData.Add("Email", author.Email);
            

            // Followers
            var followersList = await _nooterService.GetFollowedAuthors(author.AuthorId);
            personalData.Add("Followers", followersList);

            // Cheeps
            var cheepsList = await _nooterService.GetNootsWithoutPage(author.Name);
            personalData.Add("Noots", cheepsList.Select(c => c.Text)); // Kun tekst
            
            personalData.Add("Bio", Bio?.Text!);
        }

        // Returns as a JSON-file
        Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData, new JsonSerializerOptions
        {
            WriteIndented = true // For readability
        }), "application/json");
    }

    

    // This is what needs to happen when the user clicks the Forget Me! button and redirects to the page
    public async Task<IActionResult> OnPostForgetme(Author author)

    {
        var user = await _userManager.GetUserAsync(User);
        Author author1 = await _nooterService.GetAuthorByName(User.Identity?.Name!);
        
        if (string.IsNullOrEmpty(author1.ToString()))
        {
            Console.WriteLine("Has to have an author");
            return Redirect("/Identity/Account/Login");
        }
        
        await _nooterService.DeleteAuthorAndCheepsByEmail(author1.Email);
            
        // Signs out the user when it gets deleted
        await _signInManager.SignOutAsync();
        
        return RedirectToPage();
    }
    
    //This handles the request to create or update a user's bio, ensuring input validation and redirecting to the "About Me"
    //page upon success.
    public async Task<IActionResult> OnPost()
    {
        var author = await _nooterService.GetAuthorByName(User.Identity?.Name!);
        Console.WriteLine("HEJ" + author.Name);
        var user = await _userManager.GetUserAsync(User);
        if (await _nooterService.AuthorHasBio(User.Identity?.Name!))
        {
            await _nooterService.DeleteBio(author);
            Console.WriteLine("Has to have an author");
        } 

        if (string.IsNullOrWhiteSpace(BioInput?.Text))
        {
            ModelState.AddModelError("CheepInput.Text", "The message can't be empty.");
        }
        else if (BioInput.Text.Length > 300)
        {
            ModelState.AddModelError("CheepInput.Text", "The message can't be longer than 160 characters");
        }

        if (!ModelState.IsValid)
        {
            Bio = await _nooterService.GetBio(author.Name);
            return Page();
        }
        var email = user?.Email;
        
        var guid = Guid.NewGuid();
        var bioId = BitConverter.ToInt32(guid.ToByteArray(), 0);

        await _nooterService.CreateBio(User.Identity?.Name!, email!,BioInput?.Text!, bioId);
        return RedirectToPage("./_AboutMe");
    }
    
    
}