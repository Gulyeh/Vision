using System.Diagnostics;

namespace VisionClient.Core.Helpers
{
    public static class OpenBrowserHelper
    {
        public static void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
