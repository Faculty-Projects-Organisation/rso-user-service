using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace RSOUserMicroService.Pages;

public class UserUIModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int UserId { get; set; }
    public void OnGet()
    {
    }
}
