using System.Timers;
using Timer = System.Timers.Timer;

namespace YTDownloader
{
    public class DownloadManager
    {
        public DownloadManager()
        {
            CreateDirectory();
        }

        public void Start()
        {
            var timer = new Timer();
            timer.Interval = 24 * 60 * 60 * 1000;
            timer.Elapsed += downloadElapsed;
            timer.Start();
        }

        private void downloadElapsed(object? sender, ElapsedEventArgs e)
        {
            var di = new DirectoryInfo("Downloads");

            foreach (var file in di.GetFiles())
            {
                file.Delete(); 
            }
            
            foreach (var dir in di.GetDirectories())
            {
                dir.Delete(true); 
            }
            CreateDirectory();
        }
        
        private void CreateDirectory()
        {
            Directory.CreateDirectory("Downloads");;
        }
    }
}