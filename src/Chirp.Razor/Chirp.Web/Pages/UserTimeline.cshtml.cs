using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    public List<CheepDTO> Cheeps { get; set; } = null!;
    private int _page;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICheepRepository _cheepRepository;
    private Dictionary<string, bool> _followerMap;


    public UserTimelineModel(ICheepService cheepService, UserManager<ApplicationUser> userManager,ICheepRepository cheepRepository )
    {
        _cheepService = cheepService;
        _userManager = userManager;
        _cheepRepository = cheepRepository;
        _followerMap = new Dictionary<string, bool>();
    }
    
    public async Task<IActionResult> OnPostDeleteCheepAsync(int cheepId)
    {
        await _cheepRepository.DeleteCheep(cheepId);
        return RedirectToPage(new { author = RouteData.Values["author"] });
    }

    [BindProperty]
    public NootBoxModel CheepInput { get; set; }

    public async Task<ActionResult> OnGet(string author)
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]!) > 0)
        {
            _page = int.Parse(Request.Query["page"]!) -1;
        }
        
        if (User.Identity!.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            await _cheepService.CheckFollowerExistElseCreate(user!);
            Cheeps = await GetCheepsWhenLoggedIn(author);
        }
        else
        {
            Cheeps = await _cheepService.GetCheepsFromAuthor(author, _page);
        }
        ViewData["FollowerMap"] = _followerMap;
        

        return Page();
    }
    public async Task<IActionResult> OnPost()
    {
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

        await _cheepService.CreateCheep(User.Identity?.Name!, email!,CheepInput.Text, DateTimeKind.Local.ToString(), cheepId);
        return RedirectToPage("Public");
    }

    public static int ConvertGuidToInt(Guid guid)
    {
        return BitConverter.ToInt32(guid.ToByteArray(), 0);
    }
    public async Task<IActionResult> OnPostFollow(int followingAuthorId, string followerAuthor, int page)
    {
        Author author = await _cheepService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            return Redirect($"/{author.Name}");
        }
        
        await _cheepRepository.FollowAuthor(id,followingAuthorId);
        _followerMap[author.Name] = true;
        return Redirect($"/{author.Name}");
    }
    public async Task<IActionResult> OnPostUnfollow(int followingAuthorId, string followerAuthor, int page)
    {
        Author author = await _cheepService.GetAuthorByName(followerAuthor);
        int id = author.AuthorId;
        
        if (string.IsNullOrEmpty(followerAuthor))
        {
            ModelState.AddModelError("", "User identity is not valid.");
            return Redirect($"/{author.Name}");
        }
        
        
        await _cheepRepository.Unfollow(id,followingAuthorId);
        _followerMap[author.Name] = false;
        return Redirect($"/{author.Name}");
    }

    public async Task<List<CheepDTO>> GetCheepsWhenLoggedIn(string author)
    {
        var user = await _userManager.GetUserAsync(User);
        Author _author = await _cheepService.GetAuthorByName(User.Identity.Name);
        int id = _author.AuthorId;
        if (user.UserName != author)
        {
            Cheeps = await _cheepService.GetCheepsFromAuthor(author, _page);
            foreach (var cheep in Cheeps)
            {
                _followerMap[cheep.Author] = await _cheepService.IsFollowing(id, cheep.AuthorId);
            }

            return Cheeps;
        }
        else
        {
            var currentAuthor = await _cheepService.GetAuthorByName(user.UserName);
            var followedAuthors = await _cheepService.GetFollowedAuthors(currentAuthor.AuthorId);
            followedAuthors.Add(user.UserName);
        
            Cheeps = await _cheepService.GetCheepsFromFollowedAuthor(followedAuthors, _page);
            foreach (var cheep in Cheeps)
            {
                _followerMap[cheep.Author] = await _cheepService.IsFollowing(id, cheep.AuthorId);
            }

            return Cheeps;
        }
    }
    
    
}
