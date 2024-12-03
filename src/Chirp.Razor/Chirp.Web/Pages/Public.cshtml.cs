using Azure;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Chirp.Web.Pages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IFollowRepository _followRepository;
    public required List<CheepDTO> Cheeps { get; set; }
    private int _page;
    private readonly UserManager<ApplicationUser> _userManager;
    private Dictionary<string, bool> FollowerMap;

    [BindProperty]
    public required NootBoxModel CheepInput { get; set; }
    

    public PublicModel(ICheepService cheepService, UserManager<ApplicationUser> userManager, IFollowRepository followRepository)
    {
        _cheepService = cheepService;
        _userManager = userManager;
        FollowerMap = new Dictionary<string, bool>();
        _followRepository = followRepository;
    }
    

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]!) > 0)
        {
            _page = int.Parse(Request.Query["page"]!) -1;
        }
        
        Cheeps = await _cheepService.GetCheeps(_page);

        if (User.Identity?.Name == null)
        {
            ModelState.AddModelError("", "User identity is not valid.");
        }
        

        if (User.Identity!.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            await _cheepService.CheckFollowerExistElseCreate(user!);
            

            Author author = await _cheepService.GetAuthorByName(User.Identity.Name!);
            int id = author.AuthorId;
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
        var email = user!.Email;
        
        var guid = Guid.NewGuid();
        var cheepId = BitConverter.ToInt32(guid.ToByteArray(), 0);

        await _cheepService.CreateCheep(User.Identity?.Name!,email! ,CheepInput.Text, DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), cheepId);
        return RedirectToPage("Public");
    }

    public async Task<IActionResult> OnPostFollow(int followingAuthorId, string followerAuthor, int page)
    {
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            return Redirect($"/?page={page}");
        }
        
        Author author = await _cheepService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        await _followRepository.FollowAuthor(id,followingAuthorId);
        FollowerMap[author.Name] = true;
        return Redirect($"/?page={page}");
    }
    public async Task<IActionResult> OnPostUnfollow(int followingAuthorId, string followerAuthor, int page)
    {
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            return Redirect($"/?page={page}");
        }
        
        Author author = await _cheepService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        Console.WriteLine(followingAuthorId);
        await _followRepository.Unfollow(id,followingAuthorId);
        FollowerMap[author.Name] = false;
        return Redirect($"/?page={page}");
    }
    
}
