using System;
using System.Collections.Generic;
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
        /// <inheritdoc />
        /// <summary>
        ///  Initializes a single viewModel by the item in the position.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="viewModel">Model</param>
        public void InitModel(int position, IViewModel viewModel)
        {
            var line = TaskManager.Tasks[position];
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
                    // ignored
                }
            }          
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