using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Persistence.Entity;

namespace OverLayApplicationSearch.WpfApp.Pages
{
    public interface PageableWindow
    {
        /// <summary>
        /// Moves a page back
        /// </summary>
        void Back();

        /// <summary>
        /// Moves to the next page
        /// </summary>
        void Next();

        /// <summary>
        /// Starts scanning the configured task. 
        /// </summary>
        /// <param name="task"></param>
        /// <returns>scanningAlredy</returns>
       void Scan(IConfiguredTask task);

        /// <summary>
        /// Returns a nullable scanner if the window is currently scanning a task.
        /// </summary>
        IScanner Scanner { get; }

        /// <summary>
        /// Running task
        /// </summary>
        IConfiguredTask RunningTask { get; }
    }
}