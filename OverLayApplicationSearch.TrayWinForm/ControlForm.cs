using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using OverLayApplicationSearch.WpfApp;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using OverLayApplicationSearch.WpfApp.Views;

namespace OverLayApplicationSearch.TrayWinForm
{
    public partial class ControlForm : Form
    {
        public SettingsWindow SettingsWindow { get; set; }

        public ControlWindow ControlWindow { get; set; }

        public ControlForm()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.logo;
            ChangeUpdaterLocation();
        }

        private void ChangeUpdaterLocation()
        {
         //   ControlWindow = new ControlWindow();
         SettingsWindow = new SettingsWindow();
            try
            {
                if (File.Exists("NewUpdate.exe"))
                {
                    if (File.Exists("Update.exe"))
                    {
                        File.Delete("Update.exe");
                    }
                    File.Move("NewUpdate.exe", "Update.exe");
                }
                RegistryKey registryKey =
                    Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                registryKey.SetValue(Assembly.GetExecutingAssembly().GetName().Name, Application.ExecutablePath);
            }
            catch (IOException)
            {
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
      //      ElementHost.EnableModelessKeyboardInterop(ControlWindow);
       //     ControlWindow.Show();
       SettingsWindow.Show();
        }

        private void exitToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ShutDown();
        }

        private void aboutToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("Update.exe", "update-now");
                ShutDown();
            }
            catch (Exception)
            {
            }
        }

        private void ShutDown()
        {
            ControlWindow.Destroy();
            this.Close();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void ControlForm_Load(object sender, EventArgs e)
        {
            this.trayIcon.Visible = true;
            this.trayIcon.Icon = OverLayApplicationSearch.TrayWinForm.Properties.Resources.logo;
            this.trayIcon.Text = "ButterflyFinder";
            this.ShowInTaskbar = false;
            Opacity = 0;
            BeginInvoke(new MethodInvoker(Hide));
            var window = new MainWindow();
            ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();
            this.trayIcon.ShowBalloonTip(8000, "ButterflyFinder",
                "Butterflyfinder is running. Access the context menu by using the shortcut.", ToolTipIcon.Info);
        }

        private void onTrayIconKlick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //  ElementHost.EnableModelessKeyboardInterop(ControlWindow);
            // ControlWindow.Show();
            SettingsWindow.Show();

        }
    }
}