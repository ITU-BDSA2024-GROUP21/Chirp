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
    private readonly ICheepService _cheepService;
    private readonly ICheepRepository _cheepRepository;

    [BindProperty]
    public required string? Email { get; set; }
    public required List<String> FollowersList { get; set; }
    public required string Followers { get; set; }
    public required List<Cheep> CheepsList { get; set; }
    public  required List<string> CheepsListString;
    public required string Cheeps { get; set; }
    public BioDTO Bio { get; set; }

    public AboutMeModel(ICheepRepository cheepRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ICheepService cheepService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _cheepService = cheepService;
        CheepsListString = new List<string>();
        _cheepRepository = cheepRepository;
    }

    [BindProperty]
    public BioBoxModel BioInput { get; set; }
    
    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }
        
        //Loading The users email adress
        Author author = await _cheepService.GetAuthorByName(User.Identity?.Name!);
        Email = author.Email;

        if (await _cheepRepository.AuthorHasBio(author.Name))
        {
            Bio = await _cheepService.GetBio(User.Identity?.Name!);
        }

        int authorid = author.AuthorId;
        
        //loading who our uses is following
        FollowersList = await _cheepRepository.GetFollowedAuthorsAsync(authorid);
        Followers = string.Join( ", ", FollowersList.ToArray() );
        
        //loading all the cheeps
        CheepsList = await _cheepRepository.GetCheepsFromAuthor1(author.Name);

        foreach (Cheep cheep in CheepsList)
        {
            string cheepo = cheep.Text;
            Console.WriteLine("Hello " + cheepo);
            CheepsListString.Add(cheepo);
            
        }
        
        Cheeps = string.Join(", ", CheepsListString.ToArray() );

        return Page();
    }
    public async Task<IActionResult> OnPostDownload()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var personalData = new Dictionary<string, object>();

        
        var author = await _cheepService.GetAuthorByName(User.Identity?.Name!);
        if (author != null!)
        {
            personalData.Add("Name", author.Name);
            personalData.Add("Email", author.Email);
            

            // Følgere
            var followersList = await _cheepRepository.GetFollowedAuthorsAsync(author.AuthorId);
            personalData.Add("Followers", followersList);

            // Cheeps
            var cheepsList = await _cheepRepository.GetCheepsFromAuthor1(author.Name);
            personalData.Add("Noots", cheepsList.Select(c => c.Text)); // Kun tekst
        }

        // Returner som JSON-fil
        Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
        return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData, new JsonSerializerOptions
        {
            WriteIndented = true // For læsbarhed
        }), "application/json");
    }

    

    public async Task<IActionResult> OnPostForgetme(Author author)

    {
        var user = await _userManager.GetUserAsync(User);
        Author author1 = await _cheepService.GetAuthorByName(User.Identity?.Name!);
        
        if (string.IsNullOrEmpty(author1.ToString()))
        {
            Console.WriteLine("Has to have an author");
            return Redirect("/Identity/Account/Login");
        }
        
        await _cheepService.DeleteAuthorAndCheepsByEmail(author1.Email);
            
        await _signInManager.SignOutAsync();
        
        return RedirectToPage();
    }
    
    public async Task<IActionResult> OnPost()
    {
        var author = await _cheepService.GetAuthorByName(User.Identity?.Name!);
        Console.WriteLine("HEJ" + author.Name);
        var user = await _userManager.GetUserAsync(User);
        if (await _cheepRepository.AuthorHasBio(User.Identity?.Name!))
        {
            await _cheepRepository.DeleteBio(author);
            Console.WriteLine("Has to have an author");
        } 

        if (string.IsNullOrWhiteSpace(BioInput.Text))
        {
            ModelState.AddModelError("CheepInput.Text", "The message can't be empty.");
        }
        else if (BioInput.Text.Length > 300)
        {
            ModelState.AddModelError("CheepInput.Text", "The message can't be longer than 160 characters");
        }

        if (!ModelState.IsValid)
        {
            Bio = await _cheepService.GetBio(author.Name);
            return Page();
        }
        var email = user.Email;
        
        var guid = Guid.NewGuid();
        var bioId = BitConverter.ToInt32(guid.ToByteArray(), 0);

        await _cheepService.CreateBIO(User.Identity?.Name!, email!,BioInput.Text, bioId);
        return RedirectToPage("./_AboutMe");
    }
    
    
}