﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    public List<CheepDTO> Cheeps { get; set; } = null!;
    private int _page;

    public UserTimelineModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
    }

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

        await _cheepService.CreateCheep(User.Identity.Name.ToString(), CheepInput.Text, DateTime.Now.ToString());
        return RedirectToPage("Public");
    }
}
