using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;


namespace Chirp.Web.Pages;

public class AboutMeModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    [BindProperty]
    public Author Author { get; set; }
    public string? Email { get; set; }

    public AboutMeModel(ICheepRepository cheepRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _cheepRepository = cheepRepository;
        _signInManager = signInManager;
        _userManager = userManager;
    }

   
    
    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        Email = user.Email;

        return Page();
    }

    public async Task<IActionResult> OnPostForgetme(Author author)
    {
        Console.WriteLine("knap");
        if (string.IsNullOrEmpty(author.ToString()))
        {
            Console.WriteLine("Has to have an author");
            return RedirectToPage();
        }
        
        Console.WriteLine("Lort");
        await _cheepRepository.DeleteAuthorAndCheeps(author);
            
        await _signInManager.SignOutAsync();
        
        return RedirectToPage();
    }
    
    
}