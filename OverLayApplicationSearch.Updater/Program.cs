using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OverLayApplicationSearch.Updater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].Equals("update-now"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new UpdateControlWindow());
            }
            else
            {
                MessageBox.Show("Please launch this program only from butterflyfinder.exe!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
