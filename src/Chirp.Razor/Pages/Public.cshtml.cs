using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    public required List<CheepDTO> Cheeps { get; set; }
    private int _page;

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
}
