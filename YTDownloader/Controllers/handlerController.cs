using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;

namespace YTDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class handlerController : ControllerBase
    {     
        private static readonly YoutubeClient Client = new();
        
        [HttpGet("info")]
        public async Task<IActionResult> GetInfo(string url)
        {
            try
            {
                var videoInfo = await Client.Videos.GetAsync(url);
                var channelInfo = await Client.Channels.GetAsync(videoInfo.Author.ChannelUrl);
                return Ok(new DataResponse(videoInfo.Title, videoInfo.Author.ChannelTitle,
                    channelInfo.Thumbnails.MaxBy(thumbnail => thumbnail.Resolution.Area)!.Url,
                    videoInfo.Thumbnails.MaxBy(thumbnail => thumbnail.Resolution.Area)!.Url));
            }
            catch
            {
                return BadRequest("Invalid YouTube Video");
            }
        }

        private class DataResponse
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string AuthorUrl { get; set; }
            public string ThumbnailUrl { get; set; }

            public DataResponse(string title, string author, string authorUrl, string thumbnailUrl)
            {
                Title = title;
                Author = author;
                AuthorUrl = authorUrl;
                ThumbnailUrl = thumbnailUrl;
            }
        }
    }
}
