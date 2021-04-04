using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MiniMvvm.Framework.Contracts;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WinTaskKiller.WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;
            WindowState = WindowState.Maximized;

            if (IsAlreadyRunning())
            {
                MessageBox.Show("Another instance of the app is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Hide();
            PreviewKeyDown += HandleEsc;
            var startup = new Startup();
            startup.LoadDependencies(this);
            var navigator = startup.Resolve<INavigator>();
            await startup.OnStart(navigator);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
            {
                return;
            }

            if (IsVisible)
            {
                Hide();
            }
        }

        private bool IsAlreadyRunning()
        {
            try
            {
                Process curr = Process.GetCurrentProcess();
                Process[] procs = Process.GetProcessesByName(curr.ProcessName);
                foreach (Process p in procs)
                {
                    if ((p.Id != curr.Id) &&
                        (p.MainModule.FileName == curr.MainModule.FileName))
                        return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
        }
    }
}