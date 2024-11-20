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

    public UserTimelineModel(ICheepService cheepService, UserManager<ApplicationUser> userManager,ICheepRepository cheepRepository )
    {
        _cheepService = cheepService;
        _userManager = userManager;
        _cheepRepository = cheepRepository;
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
        Cheeps = await _cheepService.GetCheepsFromAuthor(author, _page);

        return Page();
    }
    public async Task<IActionResult> OnPost()
    {
        
        Console.WriteLine("HEJSA");
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

        await _cheepService.CreateCheep(User.Identity.Name.ToString(), email.ToString(),CheepInput.Text, DateTimeKind.Local.ToString(), cheepId);
        return RedirectToPage("Public");
    }

    public static int ConvertGuidToInt(Guid guid)
    {
        return BitConverter.ToInt32(guid.ToByteArray(), 0);
    }
    
    
}
