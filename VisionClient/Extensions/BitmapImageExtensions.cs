using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace VisionClient.Extensions
{
    internal static class BitmapImageExtensions
    {
        internal static string GetBase64(this BitmapImage image)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream ms = new())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return Convert.ToBase64String(data);
        }
    }
}
