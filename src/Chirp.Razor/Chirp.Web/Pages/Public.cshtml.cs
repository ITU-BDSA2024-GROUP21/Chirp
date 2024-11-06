using Azure;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

using System.ComponentModel.DataAnnotations;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    public required List<CheepDTO> Cheeps { get; set; }
    private int _page;
    
    [BindProperty]
    public NootBoxModel CheepInput { get; set; }
    

    public PublicModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
        
    }

    public async Task<ActionResult> OnGet()
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]!) > 0)
        {
            _page = int.Parse(Request.Query["page"]!) -1;
        }
        
        Cheeps = await _cheepService.GetCheeps(_page);
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

        await _cheepService.CreateCheep(User.Identity.Name.ToString(), CheepInput.Text, DateTime.Now.AddHours(1).ToString());
        return RedirectToPage("Public");
    }
    
}
