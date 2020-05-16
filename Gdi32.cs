namespace Wermwerx.Capture
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    //[SuppressUnmanagedCodeSecurity]
    internal class Gdi32
    {
        public const int SRCCOPY = 0xcc0020;

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
        //[DllImport("gdi32.dll")]
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0xcc0020,
            SRCPAINT = 0xee0086,
            SRCAND = 0x8800c6,
            SRCINVERT = 0x660046,
            SRCERASE = 0x440328,
            NOTSRCCOPY = 0x330008,
            NOTSRCERASE = 0x1100a6,
            MERGECOPY = 0xc000ca,
            MERGEPAINT = 0xbb0226,
            PATCOPY = 0xf00021,
            PATPAINT = 0xfb0a09,
            PATINVERT = 0x5a0049,
            DSTINVERT = 0x550009,
            BLACKNESS = 0x42,
            WHITENESS = 0xff0062,
            CAPTUREBLT = 0x40000000
        }
    }
}

