using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly ICheepService _cheepService;
    public List<CheepDTO> Cheeps { get; set; }
    private int page = 0;

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResult> OnGet(string author)
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]) > 0)
        {
            page = int.Parse(Request.Query["page"]) -1;
        }
        Cheeps = await _cheepService.GetCheepsFromAuthor(author, page*32);

        return Page();
    }
}
