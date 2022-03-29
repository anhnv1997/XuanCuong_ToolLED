using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using iParking;

namespace ToolLed_XuanCuong
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;

            Mutex m = new Mutex(true, "myApp", out createdNew);
            if (!createdNew)
            {
                // myApp is already running...
                LogHelper.Logger_Error("Led Controls is already running!- Multiple Instances");
                return;
            }
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLedSetting());
        }
    }
}
