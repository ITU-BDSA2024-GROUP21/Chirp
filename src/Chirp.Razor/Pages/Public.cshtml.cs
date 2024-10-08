using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepViewModel> Cheeps { get; set; }

    private int page = 0;

    public PublicModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public ActionResult OnGet()
    {
        if (!string.IsNullOrEmpty(Request.Query["page"]) && int.Parse(Request.Query["page"]) > 0)
        {
            page = int.Parse(Request.Query["page"]) -1;
        }
        Cheeps = _repository.GetCheeps(page * 32).ToList();
        return Page();
    }
}
