using System.Windows;
using MiniMvvm.Framework.Contracts;

namespace WinTaskKiller.WpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
       //     Topmost = true;
       //     WindowState = WindowState.Maximized;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var startup = new Startup();
            startup.LoadDependencies(this);
            var navigator = startup.Resolve<INavigator>();
            await startup.OnStart(navigator);
        }
    }
}
