using System.Collections.Generic;
using System.Threading.Tasks;
using WinTask = WinTaskKiller.Logic.Entity.WinTask;

namespace WinTaskKiller.Logic.Contract
{
    public interface IWinTaskService
    {
        /// <summary>
        /// Gets all current windows tasks running on the system.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/></returns>
        Task<List<WinTask>> GetAll();

        /// <summary>
        /// Kills the given <paramref name="winTask"/>.
        /// </summary>
        /// <param name="winTask"><see cref="WinTask"/> to be killed.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        Task Kill(WinTask winTask);
    }
}