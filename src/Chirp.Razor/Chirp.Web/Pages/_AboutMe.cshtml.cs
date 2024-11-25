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
    [BindProperty]
    public Author Author { get; set; }

    public AboutMeModel(ICheepRepository cheepRepository, SignInManager<ApplicationUser> signInManager)
    {
        _cheepRepository = cheepRepository;
        _signInManager = signInManager;
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