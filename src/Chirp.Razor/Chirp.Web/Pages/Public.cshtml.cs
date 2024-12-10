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
    private readonly INooterService _nooterService;
    public required List<CheepDTO> Cheeps { get; set; }
    private int _page;
    private readonly UserManager<ApplicationUser> _userManager;
    private Dictionary<string, bool> FollowerMap;

    [BindProperty]
    public required NootBoxModel CheepInput { get; set; }
    

    public PublicModel(INooterService nooterService, UserManager<ApplicationUser> userManager)
    {
        _nooterService = nooterService;
        _userManager = userManager;
        FollowerMap = new Dictionary<string, bool>();
    }
    

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]!) > 0)
        {
            _page = int.Parse(Request.Query["page"]!) -1;
        }
        
        Cheeps = await _nooterService.GetCheeps(_page);

        if (User.Identity?.Name == null)
        {
            ModelState.AddModelError("", "User identity is not valid.");
        }
        

        if (User.Identity!.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            await _nooterService.CheckFollowerExistElseCreate(user!);
            

            Author author = await _nooterService.GetAuthorByName(User.Identity.Name!);
            int id = author.AuthorId;
            foreach (var cheep in Cheeps)
            {
                FollowerMap[cheep.Author] = await _nooterService.IsFollowing(id, cheep.AuthorId);
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
            Cheeps = await _nooterService.GetCheeps(1);
            return Page();
        }
        
        var user = await _userManager.GetUserAsync(User);
        var email = user!.Email;
        
        var guid = Guid.NewGuid();
        var cheepId = BitConverter.ToInt32(guid.ToByteArray(), 0);

        await _nooterService.CreateCheep(User.Identity?.Name!,email! ,CheepInput.Text, DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), cheepId);
        return RedirectToPage("Public");
    }

    public async Task<IActionResult> OnPostFollow(int followingAuthorId, string followerAuthor, int page)
    {
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            return Redirect($"/?page={page}");
        }
        
        Author author = await _nooterService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        await _nooterService.Follow(id,followingAuthorId);
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
        
        Author author = await _nooterService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        Console.WriteLine(followingAuthorId);
        await _nooterService.Unfollow(id,followingAuthorId);
        FollowerMap[author.Name] = false;
        return Redirect($"/?page={page}");
    }
    
}
