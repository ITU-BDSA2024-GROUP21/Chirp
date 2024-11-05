using Chirp.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class NootBoxModel : PageModel {
    
    [BindProperty]
    public string Text { get; set; }
    
}