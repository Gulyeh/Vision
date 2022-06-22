using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisionClient.Utility
{
    internal static class FileDialogHelper
    {
        public static IEnumerable<BitmapImage> OpenFile(bool IsMultiselect)
        {
            OpenFileDialog op = new();
            List<BitmapImage>? ImageSource = new List<BitmapImage>();
            op.Multiselect = IsMultiselect;
            op.Filter = "JPG(*.jpg, *.jpeg) | *.jpg; *.jpeg; |PNG(*.png) | *.png;";
            if (op.ShowDialog() == true && op.FileNames.Length < 4)
            {
                foreach (var file in op.FileNames)
                {
                    FileInfo fi = new(file);
                    if (fi.Length > 3000000 || (!fi.Extension.Equals(".png") && !fi.Extension.Equals(".jpg") && !fi.Extension.Equals(".jpeg"))) continue;
                    ImageSource.Add(BitmapImageFromFile(op.FileName));
                }
            }
            return ImageSource;
        }

        public async static Task SaveFile(string url, bool copyToClipboard)
        {
            SaveFileDialog saveFileDialog = new();
            if (!copyToClipboard)
            {
                saveFileDialog.Filter = "JPG(*.jpg, *.jpeg) | *.jpg; *.jpeg |PNG(*.png) | *.png";
                saveFileDialog.ShowDialog();
            }
            HttpClient client = new();
            Stream stream = await client.GetStreamAsync(url);
            Bitmap bitmap = new Bitmap(stream);
            if (bitmap is not null)
            {
                if (!copyToClipboard) bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                else Clipboard.SetDataObject(bitmap);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }

        private static BitmapImage BitmapImageFromFile(string filepath)
        {
            var bi = new BitmapImage();

            using (var fs = new FileStream(filepath, FileMode.Open))
            {
                bi.BeginInit();
                bi.StreamSource = fs;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
            }

            bi.Freeze();

            return bi;
        }
    }
}
