namespace Wermwerx.Capture
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal class User32
    {
        private static List<IntPtr> _results = new List<IntPtr>();

        [DllImport("user32.Dll")]
        private static extern int EnumWindows(EnumWindowsProc x, int y);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        public static IntPtr[] GetWindowHandlesForThread(int threadHandle)
        {
            EnumWindows(new EnumWindowsProc(User32.WindowEnum), threadHandle);
            return _results.ToArray();
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
        private static int WindowEnum(IntPtr hWnd, int lParam)
        {
            int processId = 0;
            if (GetWindowThreadProcessId(hWnd, out processId) == lParam)
            {
                _results.Add(hWnd);
            }
            return 1;
        }

        private delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        internal enum WindowShowStyle : uint
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            Maximize = 3,
            ShowNormalNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActivate = 7,
            ShowNoActivate = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimized = 11
        }
    }
}

