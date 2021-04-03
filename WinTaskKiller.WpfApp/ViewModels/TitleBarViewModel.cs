using MiniMvvm.Framework;
using MiniMvvm.Framework.Contracts;
using WinTaskKiller.WpfApp.Contracts;

namespace WinTaskKiller.WpfApp.ViewModels
{
    public class TitleBarViewModel : ViewModel, ITitleBarViewModel
    {
        public TitleBarViewModel(IStartup startup) : base(startup)
        {
        }
    }
}