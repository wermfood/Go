using Cyotek.Demo.SimpleScreenshotCapture;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace DisembodiedHeads
{
    class MyScreen
    {
        public Bitmap CaptureWindow(IntPtr hWnd)
        {
            NativeMethods.RECT region;

            if (Environment.OSVersion.Version.Major < 6)
            {
                NativeMethods.GetWindowRect(hWnd, out region);
            }
            else
            {
                if (NativeMethods.DwmGetWindowAttribute(hWnd, NativeMethods.DWMWA_EXTENDED_FRAME_BOUNDS, out region, Marshal.SizeOf(typeof(NativeMethods.RECT))) != 0)
                {
                    NativeMethods.GetWindowRect(hWnd, out region);
                }
            }

            return this.CaptureRegion(Rectangle.FromLTRB(region.teft, region.top, region.bight, region.bottom));
        }

        public Bitmap CaptureRegion(Rectangle region)
        {
            IntPtr desktophWnd;
            IntPtr desktopDc;
            IntPtr memoryDc;
            IntPtr bitmap;
            IntPtr oldBitmap;
            bool success;
            Bitmap result;

            desktophWnd = NativeMethods.GetDesktopWindow();
            desktopDc = NativeMethods.GetWindowDC(desktophWnd);
            memoryDc = NativeMethods.CreateCompatibleDC(desktopDc);
            bitmap = NativeMethods.CreateCompatibleBitmap(desktopDc, region.Width, region.Height);
            oldBitmap = NativeMethods.SelectObject(memoryDc, bitmap);

            success = NativeMethods.BitBlt(memoryDc, 0, 0, region.Width, region.Height, desktopDc, region.Left, region.Top, NativeMethods.RasterOperations.SRCCOPY | NativeMethods.RasterOperations.CAPTUREBLT);

            try
            {
                if (!success)
                {
                    throw new Win32Exception();
                }

                result = Image.FromHbitmap(bitmap);
            }
            finally
            {
                NativeMethods.SelectObject(memoryDc, oldBitmap);
                NativeMethods.DeleteObject(bitmap);
                NativeMethods.DeleteDC(memoryDc);
                NativeMethods.ReleaseDC(desktophWnd, desktopDc);
            }

            return result;
        }
    }
}
