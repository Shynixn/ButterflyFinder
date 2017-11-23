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

namespace OverLayApplicationSearch.TrayWinForm
{
    public partial class ControlForm : Form
    {
        public KeyboardHook KeyboardHook { get; set; }

        public ControlWindow ControlWindow { get; set; }

        public ControlForm()
        {
            InitializeComponent();
            ChangeUpdaterLocation();
        }

        private void ChangeUpdaterLocation()
        {
            ControlWindow = new ControlWindow();
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
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                registryKey.SetValue(Assembly.GetExecutingAssembly().GetName().Name, Application.ExecutablePath);
            }
            catch (IOException)
            {

            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ElementHost.EnableModelessKeyboardInterop(ControlWindow);
            ControlWindow.Show();
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
            this.trayIcon.Text = "OverLayApplicationSearch";
            this.ShowInTaskbar = false;
            Opacity = 0;
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
            }));
            var window = new MainWindow();
            ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();

        }
    }
}
