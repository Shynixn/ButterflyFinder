using System.Windows;
using System.Windows.Input;
using MiniMvvm.Framework.Contracts;

namespace WinTaskKiller.WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;
            WindowState = WindowState.Maximized;
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
    }
}