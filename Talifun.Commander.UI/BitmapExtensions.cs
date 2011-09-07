using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Talifun.Commander.UI
{
    public static class BitmapExtensions
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(24, 24));

            DeleteObject(hBitmap);

            return bitmapSource;
        }
    }
}
