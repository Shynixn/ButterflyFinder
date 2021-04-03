using System.Collections.Generic;
using System.Threading.Tasks;
using WinTaskKiller.Logic.Entity;

namespace WinTaskKiller.WpfApp.Contracts
{
    public interface IWinTasksModel
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
