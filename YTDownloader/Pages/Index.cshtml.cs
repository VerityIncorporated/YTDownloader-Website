using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YTDownloader.Pages;

public class Index : PageModel
{
    [BindProperty]
    public string? url { get; set; }
}

