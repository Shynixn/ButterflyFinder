using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MiniMvvm.Framework;
using MiniMvvm.Framework.Contracts;
using WinTaskKiller.Logic.Entity;
using WinTaskKiller.WpfApp.Contracts;
using WinTaskKiller.WpfApp.Views;

namespace WinTaskKiller.WpfApp.ViewModels
{
    public class WinTasksViewModel : ViewModel<IWinTasksModel>, IWinTasksViewModel
    {
        /// <summary>
        /// List of displayed wintasks.
        /// </summary>
        public ObservableCollection<WinTask> WinTasks { get; set; }

        /// <summary>
        /// Command which gets executed when a task should be killed.
        /// </summary>
        public IActionCommand<WinTask> KillTaskCommand { get; }

        public WinTasksViewModel(IStartup startup) : base(startup)
        {
            KillTaskCommand = new ActionCommand<WinTask>(KillTask);

            // Some Things do not work well without code behind.
            ((WinTasksView) View).FocusListBox();
            ((WinTasksView) View).OnTaskKillEvent += async kill => await KillTask(kill);
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

        /// <summary>
        /// Kills the given tasks and reloads all visible tasks.
        /// </summary>
        /// <param name="task">Task to be killed.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        private async Task KillTask(WinTask task)
        {
            try
            {
                await Model.Kill(task);
            }
            catch (Exception)
            {
                //Ignored.
            }

            await OnLoadAsync();
        }
    }
}