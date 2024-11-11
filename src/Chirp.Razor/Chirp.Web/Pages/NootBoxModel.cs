using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class NootBoxModel {
    
    [BindProperty]
    [Microsoft.Build.Framework.Required]
    [StringLength(160, ErrorMessage = "Maximum length is {1}")]
    [Display(Name = "Message Text")]

    public required string Text { get; set; }

}