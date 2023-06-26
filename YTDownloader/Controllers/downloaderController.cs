using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace YTDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class downloaderController : ControllerBase
    {
        private static readonly YoutubeClient Client = new();
        
        [HttpGet("video")]
        public async Task<IActionResult> DownloadVideo(string url)
        {
            try
            {
                var video = await Client.Videos.GetAsync(url);
                var getChannel = await Client.Channels.GetAsync(video.Author.ChannelUrl);

                var filePath = $"Downloads/{getChannel.Id}/videos/{video.Id}";
                if (System.IO.File.Exists(filePath))
                {
                    var fileContents = await System.IO.File.ReadAllBytesAsync(filePath);
                    return File(fileContents, "video/mp4", $"{video.Title}");
                }
                else
                {
                    Directory.CreateDirectory($"Downloads/{getChannel.Id}/videos/");
                
                    var streamInfoSet = await Client.Videos.Streams.GetManifestAsync(video.Id);
                
                    var audioStreams = streamInfoSet.GetAudioOnlyStreams().MaxBy(info => info.Bitrate);
                    var videoStreams = streamInfoSet.GetVideoOnlyStreams().MaxBy(info => info.VideoQuality.MaxHeight == 1080);
                
                    var streamInfos = new IStreamInfo[] { videoStreams!, audioStreams! };
                    await Client.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(filePath).SetContainer(Container.Mp4).SetPreset(ConversionPreset.UltraFast).SetFFmpegPath(@"C:\ffmpeg\bin\ffmpeg.exe").Build());
            
                    var fileContents = await System.IO.File.ReadAllBytesAsync(filePath);
                    return File(fileContents, "video/mp4", $"{video.Title}");
                }
            }
            catch
            {
                return Problem();
            }
        }

        [HttpGet("audio")]
        public async Task<IActionResult> DownloadAudio(string url)
        {
            try
            {
                var video = await Client.Videos.GetAsync(url);
                var getChannel = await Client.Channels.GetAsync(video.Author.ChannelUrl);

                var filePath = $"Downloads/{getChannel.Id}/audios/{video.Id}";
                if (System.IO.File.Exists(filePath))
                {
                    var fileContents = await System.IO.File.ReadAllBytesAsync(filePath);
                    return File(fileContents, "audio/mpeg", $"{video.Title}");
                }
                else
                {
                    Directory.CreateDirectory($"Downloads/{getChannel.Id}/audios/");
                
                    var streamInfoSet = await Client.Videos.Streams.GetManifestAsync(video.Id);
                    var audioStreams = streamInfoSet.GetAudioOnlyStreams().MaxBy(info => info.Bitrate);

                    var streamInfos = new IStreamInfo[] { audioStreams! };
                    await Client.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(filePath).SetContainer(Container.Mp3).SetPreset(ConversionPreset.UltraFast).SetFFmpegPath(@"C:\ffmpeg\bin\ffmpeg.exe").Build());
            
                    var fileContents = await System.IO.File.ReadAllBytesAsync(filePath);
                    return File(fileContents, "audio/mpeg", $"{video.Title}");
                }
            }
            catch
            {
                return Problem();
            }
        }
    }
}
