using System;
using System.Windows.Forms;

namespace Csv2Xlsx3
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args != null && args.Length > 0)
            {
                MainForm.ProcessCsvFileSilent(args[0]);
                return;
            }

            Application.Run(new MainForm(args));
        }

        public static DateTime GetExpiryDate()
        {
            return new DateTime(2026, 12, 31);
        }
    }
}
