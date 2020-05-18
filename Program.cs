namespace DisembodiedHeads
{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new DisembodiedHeads.Main());
            }finally
            {

            }
        }
    }
}

