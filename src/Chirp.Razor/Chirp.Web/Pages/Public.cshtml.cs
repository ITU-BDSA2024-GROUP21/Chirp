using Azure;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly ICheepRepository _cheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }
    private int _page;
    private readonly UserManager<ApplicationUser> _userManager;
    private Dictionary<string, bool> FollowerMap;

    [BindProperty]
    public NootBoxModel CheepInput { get; set; }
    

    public PublicModel(ICheepService cheepService, UserManager<ApplicationUser> userManager, ICheepRepository cheepRepository)
    {
        _cheepService = cheepService;
        _cheepRepository = cheepRepository;
        _userManager = userManager;
        FollowerMap = new Dictionary<string, bool>();
    }
    

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]!) > 0)
        {
            _page = int.Parse(Request.Query["page"]!) -1;
        }
        
        Cheeps = await _cheepService.GetCheeps(_page);
        Author author = await _cheepService.GetAuthorByName(User.Identity.Name);
        int id = author.AuthorId;

        if (User.Identity.IsAuthenticated)
        {
            foreach (var cheep in Cheeps)
            {
                FollowerMap[cheep.Author] = await _cheepService.IsFollowing(id, cheep.AuthorId);
            }
        }
        ViewData["FollowerMap"] = FollowerMap;
        
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        Console.WriteLine("Hejsa");
        if (string.IsNullOrWhiteSpace(CheepInput.Text))
        {
            ModelState.AddModelError("CheepInput.Text", "The message can't be empty.");
        }
        else if (CheepInput.Text.Length > 160)
        {
            ModelState.AddModelError("CheepInput.Text", "The message can't be longer than 160 characters");
        }

        if (!ModelState.IsValid)
        {
            Cheeps = await _cheepService.GetCheeps(1);
            return Page();
        }
        
        var user = await _userManager.GetUserAsync(User);
        var email = user.Email;
        
        var guid = Guid.NewGuid();
        var cheepId = BitConverter.ToInt32(guid.ToByteArray(), 0);

        await _cheepService.CreateCheep(User.Identity.Name.ToString(),email.ToString() ,CheepInput.Text, DateTime.Now.AddHours(1).ToString(), cheepId);
        return RedirectToPage("Public");
    }

    public async Task<IActionResult> OnPostFollow(int followingAuthorId, string followerAuthor, int page)
    {
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            Console.WriteLine("whoopsies");
            return Redirect($"/?page={page}");
        }
        Console.WriteLine(followerAuthor);
        
        
        Author author = await _cheepService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        Console.WriteLine(followingAuthorId);
        await _cheepRepository.FollowAuthor(id,followingAuthorId);
        FollowerMap[author.Name] = true;
        return Redirect($"/?page={page}");
    }
    
}
