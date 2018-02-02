using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Business.Service;
using OverLayApplicationSearch.Logic.Lib;

namespace OverLayApplicationSearch.Logic.Business.Service
{
    public class TaskKillerService : IListService
    {

        private string[] taskCache;
        private Icon fallBackIcon;

        public TaskKillerService(Icon fallBackIcon)
        {
            this.fallBackIcon = fallBackIcon;
        }

        /// <inheritdoc />
        /// <summary>
        ///  Initializes a single viewModel by the item in the position.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="viewModel">Model</param>
        public bool InitModel(int position, IViewModel viewModel)
        {
            if (position == 0)
            {
                taskCache = TaskManager.Tasks;
            }
            var line = taskCache[position];
            if (line != null)
            {
                viewModel.Text = Path.GetFileName(line);
                viewModel.Header = line;
                try
                {
                    viewModel.Icon = line.GenerateImageSource();
                }
                catch (Exception)
                {
                    viewModel.Icon = fallBackIcon.GenerateImageSource();
                }
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Handles an action when an viewModel item is pressed or clicked.
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="viewModel">viewModel</param>
        public void OnAction(int position, IViewModel viewModel)
        {
            TaskManager.Kill(viewModel.Header);
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the amount of items.
        /// </summary>
        public int Count => TaskManager.Tasks.Length;
    }
}