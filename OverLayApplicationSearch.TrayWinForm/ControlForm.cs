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
using System.Reflection;
using OverLayApplicationSearch.Logic.Lib;
using Microsoft.Win32;
using OverLayApplicationSearch.Logic;
using OverLayApplicationSearch.Logic.Persistence.Controller;

namespace OverLayApplicationSearch.TrayWinForm
{
    public partial class ControlForm : Form
    {
        private ShellNotifications Notifications = new ShellNotifications();

        public KeyboardHook KeyboardHook { get; set; }

        public ControlWindow ControlWindow { get; set; }

        public ControlForm()
        {
            InitializeComponent();
            ChangeUpdaterLocation();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)ShellNotifications.WM_SHNOTIFY:
                    if (Notifications.NotificationReceipt(m.WParam, m.LParam))
                        NewOperation((NotifyInfos)Notifications.NotificationsReceived[Notifications.NotificationsReceived.Count - 1]);
                    break;
            }
            base.WndProc(ref m);
        }

        private async void NewOperation(NotifyInfos info)
        {
            await this.UpdateDatabaseToFileSystemChange(info);
        }

        /// <summary>
        /// Displays the settings window for the user when he clicks the icon in the trayBar.
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">e</param>
        private void onTrayIconKlick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ElementHost.EnableModelessKeyboardInterop(ControlWindow);
            ControlWindow.Show();
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
            Notifications.UnregisterChangeNotify();
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

        private void ControlForm_Load(object sender, EventArgs e)
        {
            Notifications.RegisterChangeNotify(this.Handle, ShellNotifications.CSIDL.CSIDL_DESKTOP, true);
            this.trayIcon.Visible = true;
            this.trayIcon.Icon = OverLayApplicationSearch.TrayWinForm.Properties.Resources.logo;
            this.trayIcon.Text = "ButterflyFinder";
            this.ShowInTaskbar = true;
            Opacity = 0;
            BeginInvoke(new MethodInvoker(delegate
            {
               Hide();
            }));
            var window = new MainWindow();
            ElementHost.EnableModelessKeyboardInterop(window);
            window.Show();
        }


        private Task UpdateDatabaseToFileSystemChange(NotifyInfos info)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var controller = Factory.CreateFilecacheController())
                    {
                        controller.OnFileSystemChange(info);
                    }
                }
                catch(Exception ex)
                {

                }
              
            });
        }

        /// <summary>
        /// Completely closes all instances of this program.
        /// </summary>
        private void ShutDown()
        {
            ControlWindow.Destroy();
            this.Close();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
