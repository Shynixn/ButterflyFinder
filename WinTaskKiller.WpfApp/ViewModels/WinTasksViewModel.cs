using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MiniMvvm.Framework;
using MiniMvvm.Framework.Contracts;
using WinTaskKiller.Logic.Entity;
using WinTaskKiller.WpfApp.Contracts;

namespace WinTaskKiller.WpfApp.ViewModels
{
    public class WinTasksViewModel : ViewModel<IWinTasksModel>, IWinTasksViewModel
    {
        /// <summary>
        /// List of displayed wintasks.
        /// </summary>
        public ObservableCollection<WinTask> WinTasks { get; set; }

        public WinTasksViewModel(IStartup startup) : base(startup)
        {
        }

        /// <summary>
        /// Gets called when the view is loaded.
        /// </summary>
        /// <returns></returns>
        public override async Task OnLoadAsync()
        {
           var items = await Model.GetAll();
           WinTasks = new ObservableCollection<WinTask>(items);
           OnPropertyChanged(nameof(WinTasks));
        }
    }
}