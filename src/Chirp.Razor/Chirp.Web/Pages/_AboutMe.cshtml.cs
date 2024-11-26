using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;


namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICheepService _cheepService;

    [BindProperty]
    public Author Author { get; set; }
    public string? Email { get; set; }
    List<String> FollowersList { get; set; }
    public string Followers { get; set; }
    
    public List<Cheep> CheepsList { get; set; }
    public  List<string> CheepsListString;
    public string Cheeps { get; set; }

    public AboutMeModel(ICheepRepository cheepRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ICheepService cheepService)
    {
        _cheepRepository = cheepRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _cheepService = cheepService;
        CheepsListString = new List<string>();
    }

   
    
    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Redirect("/Identity/Account/Login");
        }
        
        //Loading The users email adress
        Author author = await _cheepService.GetAuthorByName(User.Identity.Name);
        Email = author.Email;

        int Authorid = author.AuthorId;
        
        //loading who our uses is following
        FollowersList = await _cheepRepository.GetFollowedAuthorsAsync(Authorid);
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

        
        var author = await _cheepService.GetAuthorByName(User.Identity.Name);
        if (author != null)
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
        Console.WriteLine("knap");
        if (string.IsNullOrEmpty(author.ToString()))
        {
            Console.WriteLine("Has to have an author");
            return Redirect("/Identity/Account/Login");
        }
        
        Console.WriteLine("Lort");
        await _cheepRepository.DeleteAuthorAndCheeps(author);
            
        await _signInManager.SignOutAsync();
        
        return RedirectToPage();
    }
    
    
}