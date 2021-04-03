using System.Collections.ObjectModel;
using MiniMvvm.Framework.Contracts;
using WinTaskKiller.Logic.Entity;

namespace WinTaskKiller.WpfApp.Contracts
{
    public interface IWinTasksViewModel : IViewModel<IWinTasksModel>
    {
        /// <summary>
        /// List of displayed wintasks.
        /// </summary>
        ObservableCollection<WinTask> WinTasks { get; set; }
    }
}
