using System.Net;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YTDownloader.Pages;

public class Video : PageModel
{
    [BindProperty]
    public string? videoUrl { get; set; }
    
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? AuthorUrl { get; set; }
    public string? ThumbnailUrl { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (string.IsNullOrEmpty(Request.Query["URL"]))
        {
            return RedirectToPage("Index");
        }
        
        videoUrl = HttpUtility.UrlDecode(Request.Query["URL"]);
        if (!videoUrl.Contains("https://www.youtube") || !videoUrl.Contains("watch?v="))
        {
            return RedirectToPage("Index");
        }
        await GetData();
        
        return Page();
    }

    public async Task GetData()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"https://localhost:7067/api/handler/info?url={videoUrl}");
        if (response.StatusCode == HttpStatusCode.BadRequest)
        { 
            RedirectToPage("Index");
            return;
        }
        
        var rootElement = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
        Title = rootElement.GetProperty("title").ToString();
        Author = rootElement.GetProperty("author").ToString();
        AuthorUrl = rootElement.GetProperty("authorUrl").ToString();
        ThumbnailUrl = rootElement.GetProperty("thumbnailUrl").ToString();
    }
}