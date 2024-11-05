using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class NootBoxModel : PageModel {

    [BindProperty]
    public static string Text { get; set; }

    public IActionResult OnPost()
    {
        
        if (!ModelState.IsValid)
            return Page();
        return RedirectToPage("Success");
    }
    
}