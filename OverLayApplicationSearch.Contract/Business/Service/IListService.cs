using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Business.Entity;

namespace OverLayApplicationSearch.Contract.Business.Service
{
    public interface IListService : IService
    {
        /// <summary>
        ///  Initializes a single viewModel by the item in the position.
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="viewModel">Model</param>
        bool InitModel(int position, IViewModel viewModel);

        /// <summary>
        /// Handles an action when an viewModel item is pressed or clicked.
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="viewModel">viewModel</param>
        void OnAction(int position, IViewModel viewModel);

        /// <summary>
        /// Returns the amount of items.
        /// </summary>
        int Count { get; }
    }
}
