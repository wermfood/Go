namespace Wermwerx.Capture
{
    using Cyotek.Demo.SimpleScreenshotCapture;
    using System;
    using System.Drawing;

    public class ScreenCapture
    {
        private static void ApplyChromaKey(Image img)
        {
            LockBitmap bitmap2 = new LockBitmap(new Bitmap(img));
            bitmap2.LockBits();
            Color pixel = bitmap2.GetPixel(0, 0);
            Color transparent = Color.Transparent;
            int y = 0;
            while (true)
            {
                if (y >= bitmap2.Height)
                {
                    bitmap2.UnlockBits();
                    return;
                }
                int x = 0;
                while (true)
                {
                    if (x >= bitmap2.Width)
                    {
                        y++;
                        break;
                    }
                    if (ColorWithinRange(pixel, bitmap2.GetPixel(x, y)))
                    {
                        bitmap2.SetPixel(x, y, transparent);
                    }
                    x++;
                }
            }
        }

        public static Image Capture(ScreenWindow screenWindow)
        {
            Image image2;
            if (ReferenceEquals(screenWindow, null))
            {
                image2 = null;
            }
            else
            {
                IntPtr windowDC = User32.GetWindowDC(screenWindow.Handle);
                User32.RECT rect = new User32.RECT();
                User32.GetWindowRect(screenWindow.Handle, ref rect);
                int nWidth = rect.right - rect.left;
                int nHeight = rect.bottom - rect.top;
                IntPtr hObject = Gdi32.CreateCompatibleDC(windowDC);
                IntPtr ptr3 = Gdi32.CreateCompatibleBitmap(windowDC, nWidth, nHeight);
                Gdi32.BitBlt(hObject, 0, 0, nWidth, nHeight, windowDC, 0, 0, 0xcc0020);
                Gdi32.SelectObject(hObject, Gdi32.SelectObject(hObject, ptr3));
                Gdi32.DeleteDC(hObject);
                User32.ReleaseDC(screenWindow.Handle, windowDC);
                Image image = Image.FromHbitmap(ptr3);
                Gdi32.DeleteObject(ptr3);
                image2 = image;
            }
            return image2;
        }

        public static Image Capture2(ScreenWindow screenWindow)
        {
            Image image2;
            if ((screenWindow == null) || (screenWindow.Handle == IntPtr.Zero))
            {
                image2 = null;
            }
            else
            {
                IntPtr windowDC = User32.GetWindowDC(screenWindow.Handle);
                User32.RECT rect = new User32.RECT();
                User32.GetWindowRect(screenWindow.Handle, ref rect);
                int nWidth = rect.right - rect.left;
                int nHeight = rect.bottom - rect.top;
                int nXSrc = 0;
                int nYSrc = 0;
                if (((screenWindow.SelectedRectangle != Rectangle.Empty) && (screenWindow.SelectedRectangle.Width != 0)) && (screenWindow.SelectedRectangle.Height != 0))
                {
                    nWidth = screenWindow.SelectedRectangle.Width;
                    nHeight = screenWindow.SelectedRectangle.Height;
                    nXSrc = screenWindow.SelectedRectangle.X;
                    nYSrc = screenWindow.SelectedRectangle.Y;
                }
                IntPtr hObject = Gdi32.CreateCompatibleDC(windowDC);
                IntPtr ptr3 = Gdi32.CreateCompatibleBitmap(windowDC, nWidth, nHeight);
                Gdi32.BitBlt(hObject, 0, 0, nWidth, nHeight, windowDC, nXSrc, nYSrc, 0xcc0020);
                Gdi32.SelectObject(hObject, Gdi32.SelectObject(hObject, ptr3));
                Gdi32.DeleteDC(hObject);
                User32.ReleaseDC(screenWindow.Handle, windowDC);
                Image image = null;
                try
                {
                    image = Image.FromHbitmap(ptr3);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(exception.StackTrace);
                }
                Gdi32.DeleteObject(ptr3);
                image2 = image;
            }
            return image2;
        }

        public static Image Capture3(ScreenWindow screenWindow)
        {
            Image image2;
            if ((screenWindow == null) || (screenWindow.Handle == IntPtr.Zero))
            {
                image2 = null;
            }
            else
            {
                User32.RECT rect = new User32.RECT();
                User32.GetWindowRect(screenWindow.Handle, ref rect);
                int nWidth = rect.right - rect.left;
                int nHeight = rect.bottom - rect.top;
                int nXSrc = 0;
                int nYSrc = 0;
                if (((screenWindow.SelectedRectangle != Rectangle.Empty) && (screenWindow.SelectedRectangle.Width != 0)) && (screenWindow.SelectedRectangle.Height != 0))
                {
                    nWidth = screenWindow.SelectedRectangle.Width;
                    nHeight = screenWindow.SelectedRectangle.Height;
                    nXSrc = screenWindow.SelectedRectangle.X;
                    nYSrc = screenWindow.SelectedRectangle.Y;
                }

                //~IntPtr desktophWnd = NativeMethods.GetDesktopWindow();
                //~IntPtr desktopDC = NativeMethods.GetWindowDC(desktophWnd);
                IntPtr desktopDC = NativeMethods.GetWindowDC(screenWindow.Handle);
                IntPtr memoryDC = NativeMethods.CreateCompatibleDC(desktopDC);
                IntPtr bitMap = NativeMethods.CreateCompatibleBitmap(desktopDC, nWidth, nHeight);
                IntPtr oldBitmap = NativeMethods.SelectObject(memoryDC, bitMap);
                //bool success = NativeMethods.BitBlt(memoryDC, 0, 0, nWidth, nHeight, desktopDC, rect.left, rect.top, NativeMethods.RasterOperations.SRCCOPY | NativeMethods.RasterOperations.CAPTUREBLT);

                bool success = true;
                Gdi32.BitBlt(memoryDC, 0, 0, nWidth, nHeight, desktopDC, nXSrc, nYSrc, 0xcc0020);

                try
                {
                    if (!success)
                    {
                        throw new ApplicationException("Unable to process the screen");
                    }

                    image2 = Image.FromHbitmap(bitMap);
                }
                finally
                {
                    NativeMethods.SelectObject(memoryDC, oldBitmap);
                    NativeMethods.DeleteObject(bitMap);
                    NativeMethods.DeleteDC(memoryDC);
                    //~NativeMethods.ReleaseDC(desktophWnd, desktopDC);
                }
                //IntPtr windowDC = NativeMethods.GetWindowDC(screenWindow.Handle);
                //IntPtr hObject = NativeMethods.CreateCompatibleDC(windowDC);
                //IntPtr ptr3 = NativeMethods.CreateCompatibleBitmap(windowDC, 1920, 1080);
                //IntPtr oldBitmap = NativeMethods.SelectObject(hObject, ptr3);
                //NativeMethods.BitBlt(hObject, 0, 0, 1920, 1080, windowDC, 1920, 1080, NativeMethods.RasterOperations.SRCCOPY | NativeMethods.RasterOperations.CAPTUREBLT);
                //Bitmap bbb = Image.FromHbitmap(ptr3);
                //bbb.Save("c:\\hold\\craycray.bmp");

                //User32.RECT rect = new User32.RECT();
                //User32.GetWindowRect(screenWindow.Handle, ref rect);
                //int nWidth = rect.right - rect.left;
                //int nHeight = rect.bottom - rect.top;
                //int nXSrc = 0;
                //int nYSrc = 0;
                //if (((screenWindow.SelectedRectangle != Rectangle.Empty) && (screenWindow.SelectedRectangle.Width != 0)) && (screenWindow.SelectedRectangle.Height != 0))
                //{
                //    nWidth = screenWindow.SelectedRectangle.Width;
                //    nHeight = screenWindow.SelectedRectangle.Height;
                //    nXSrc = screenWindow.SelectedRectangle.X;
                //    nYSrc = screenWindow.SelectedRectangle.Y;
                //}
                //IntPtr hObject = Gdi32.CreateCompatibleDC(windowDC);
                //IntPtr ptr3 = Gdi32.CreateCompatibleBitmap(windowDC, nWidth, nHeight);
                //Gdi32.BitBlt(hObject, 0, 0, nWidth, nHeight, windowDC, nXSrc, nYSrc, 0xcc0020);
                //Gdi32.SelectObject(hObject, Gdi32.SelectObject(hObject, ptr3));
                //Gdi32.DeleteDC(hObject);
                //User32.ReleaseDC(screenWindow.Handle, windowDC);
                //Image image = null;
                //try
                //{
                //    image = Image.FromHbitmap(ptr3);
                //}
                //catch (Exception exception)
                //{
                //    Console.WriteLine(exception.Message);
                //    Console.WriteLine(exception.StackTrace);
                //}
                //Gdi32.DeleteObject(ptr3);
                //image2 = image;
            }
            //~return image2;
            //return null;
            return image2;
        }

        private static bool ColorWithinRange(Color chroma, Color c)
        {
            int num = 100;
            int num2 = Math.Max(chroma.R - num, 0);
            int num3 = Math.Min(chroma.R + num, 0xff);
            int num4 = Math.Max(chroma.G - num, 0);
            int num5 = Math.Min(chroma.G + num, 0xff);
            int num6 = Math.Max(chroma.B - num, 0);
            int num7 = Math.Min(chroma.B + num, 0xff);
            bool flag = false;
            if (((c.R >= num2) && ((c.R <= num3) && ((c.G >= num4) && (c.G <= num5)))) && ((c.B >= num6) && (c.B <= num7)))
            {
                flag = true;
            }
            return flag;
        }
    }
}

