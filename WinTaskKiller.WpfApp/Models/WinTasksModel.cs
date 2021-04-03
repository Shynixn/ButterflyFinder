using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinTaskKiller.Logic.Contract;
using WinTaskKiller.Logic.Entity;
using WinTaskKiller.WpfApp.Contracts;

namespace WinTaskKiller.WpfApp.Models
{
    public class WinTasksModel : IWinTasksModel
    {
        private IWinTaskService _winTaskService;

        /// <summary>
        ///  Creates a new instance.
        /// </summary>
        public WinTasksModel(IWinTaskService winTaskService)
        {
            this._winTaskService = winTaskService;
        }

        /// <summary>
        /// Gets all current windows tasks running on the system.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/></returns>
        public Task<List<WinTask>> GetAll()
        {
            return _winTaskService.GetAll();
        }

        /// <summary>
        /// Kills the given <paramref name="winTask"/>.
        /// </summary>
        /// <param name="winTask"><see cref="WinTask"/> to be killed.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        public Task Kill(WinTask winTask)
        {
            return _winTaskService.Kill(winTask);
        }
    }
}