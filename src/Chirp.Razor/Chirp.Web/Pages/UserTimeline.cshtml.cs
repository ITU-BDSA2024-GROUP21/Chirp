using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly INooterService _nooterService;
    public List<CheepDTO> Cheeps { get; set; } = null!;
    private int _page;
    private readonly UserManager<ApplicationUser> _userManager;
    private Dictionary<string, bool> _followerMap;
    public BioDTO? Bio { get; set; }


    public UserTimelineModel(INooterService nooterService, UserManager<ApplicationUser> userManager)
    {
        _nooterService = nooterService;
        _userManager = userManager;
        _followerMap = new Dictionary<string, bool>();
    }
    
    public async Task<IActionResult> OnPostDeleteCheepAsync(int cheepId)
    {
        await _nooterService.DeleteNoot(cheepId);
        return RedirectToPage(new { author = RouteData.Values["author"] });
    }

    [BindProperty]
    public NootBoxModel? CheepInput { get; set; }

    public async Task<ActionResult> OnGet(string author)
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]!) > 0)
        {
            _page = int.Parse(Request.Query["page"]!) -1;
        }
        
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            await _nooterService.CheckFollowerExistElseCreate(user!);
            Cheeps = await GetCheepsWhenLoggedIn(author);
            Author author1 = await _nooterService.GetAuthorByName(User.Identity?.Name!);
            if (User.Identity?.Name != author)
            {
                if (await _nooterService.AuthorHasBio(author))
                {
                    Bio = await _nooterService.GetBio(author);
                }
            }
            else
            {
                if (await _nooterService.AuthorHasBio(author1.Name))
                {
                    Bio = await _nooterService.GetBio(User.Identity?.Name!);
                }
            }

        }
        else
        {
            Cheeps = await _nooterService.GetCheepsFromAuthor(author, _page);
            if (await _nooterService.AuthorHasBio(author))
            {
                Bio = await _nooterService.GetBio(author);
            }
        }
        ViewData["FollowerMap"] = _followerMap;

        return Page();
    }
    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrWhiteSpace(CheepInput?.Text))
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
        var email = user?.Email;
        
        var guid = Guid.NewGuid();
        var cheepId = BitConverter.ToInt32(guid.ToByteArray(), 1);

        await _nooterService.CreateCheep(User.Identity?.Name!, email!,CheepInput?.Text!, DateTimeKind.Local.ToString(), cheepId);
        return RedirectToPage("Public");
    }
    
    public async Task<IActionResult> OnPostFollow(int followingAuthorId, string followerAuthor, int page)
    {
        Author author = await _nooterService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            return Redirect($"/{author.Name}");
        }
        
        await _nooterService.Follow(id,followingAuthorId);
        _followerMap[author.Name] = true;
        return Redirect($"/{author.Name}");
    }
    public async Task<IActionResult> OnPostUnfollow(int followingAuthorId, string followerAuthor, int page)
    {
        Author author = await _nooterService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            return Redirect($"/{author.Name}");
        }
        
        
        await _nooterService.Unfollow(id,followingAuthorId);
        _followerMap[author.Name] = false;
        return Redirect($"/{author.Name}");
    }

    public async Task<List<CheepDTO>> GetCheepsWhenLoggedIn(string author)
    {
        var user = await _userManager.GetUserAsync(User);
        Author loggedAuthor = await _nooterService.GetAuthorByName(User.Identity?.Name!);
        int id = loggedAuthor.AuthorId;
        if (user?.UserName != author)
        {
            Cheeps = await _nooterService.GetCheepsFromAuthor(author, _page);
            foreach (var cheep in Cheeps)
            {
                _followerMap[cheep.Author] = await _nooterService.IsFollowing(id, cheep.AuthorId);
            }

            return Cheeps;
        }
        else
        {
            var currentAuthor = await _nooterService.GetAuthorByName(user.UserName);
            var followedAuthors = await _nooterService.GetFollowedAuthors(currentAuthor.AuthorId);
            followedAuthors.Add(user.UserName);
        
            Cheeps = await _nooterService.GetCheepsFromFollowedAuthor(followedAuthors, _page);
            foreach (var cheep in Cheeps)
            {
                _followerMap[cheep.Author] = await _nooterService.IsFollowing(id, cheep.AuthorId);
            }

            return Cheeps;
        }
    }
    
    
}
