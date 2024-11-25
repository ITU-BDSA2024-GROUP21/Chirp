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

    public AboutMeModel(ICheepRepository cheepRepository, SignInManager<ApplicationUser> signInManager, Author author)
    {
        _cheepRepository = cheepRepository;
        _signInManager = signInManager;
        Author = author;
    }
    
    

    public async Task<IActionResult> OnPostForgetMe()
    {
        Console.WriteLine("knap");
        if (!string.IsNullOrEmpty(Author.ToString()))
        {
            Console.WriteLine("Lort");
            await _cheepRepository.DeleteAuthorAndCheeps(Author);
            
            await _signInManager.SignOutAsync();
        }
        
        return RedirectToPage("/?page=1");
    }
}