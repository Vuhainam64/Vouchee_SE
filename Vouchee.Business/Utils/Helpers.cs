using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Utils
{
    public static class Helpers
    {
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };

        // List of supported video extensions
        private static readonly string[] VideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".mkv", ".flv" };

        public static bool IsValidImageUrl(string url)
        {
            return IsValidUrl(url, ImageExtensions);
        }

        public static bool IsValidVideoUrl(string url)
        {
            return IsValidUrl(url, VideoExtensions);
        }
    }
}
