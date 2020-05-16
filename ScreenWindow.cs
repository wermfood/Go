namespace Wermwerx.Capture
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    public class ScreenWindow
    {
        public ScreenWindow()
        {
            this.Handle = IntPtr.Zero;
            this.Name = string.Empty;
        }

        public ScreenWindow(IntPtr handle, string name)
        {
            this.Handle = handle;
            this.Name = name;
        }

        public static List<ScreenWindow> InnerWindows(ScreenWindow screenWindow)
        {
            List<ScreenWindow> list = new List<ScreenWindow>();
            foreach (Process process in Process.GetProcessesByName(screenWindow.Name))
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    IntPtr[] windowHandlesForThread = User32.GetWindowHandlesForThread(thread.Id);
                    if ((windowHandlesForThread != null) && (windowHandlesForThread.Length != 0))
                    {
                        foreach (IntPtr ptr in windowHandlesForThread)
                        {
                            if (process.MainWindowHandle == ptr)
                            {
                                list.Add(new ScreenWindow(process.MainWindowHandle, process.ProcessName));
                            }
                        }
                    }
                }
            }
            return list;
        }

        public bool IsWindowInForeground(ScreenWindow screenWindow) => 
            (screenWindow.Handle == User32.GetForegroundWindow());

        public IntPtr Handle { get; set; }

        public string Name { get; set; }

        public Rectangle SelectedRectangle { get; set; }

        public static List<ScreenWindow> AvailableWindows
        {
            get
            {
                List<ScreenWindow> list = new List<ScreenWindow>();
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        list.Add(new ScreenWindow(process.MainWindowHandle, process.ProcessName));
                    }
                }
                return list;
            }
        }
    }
}

