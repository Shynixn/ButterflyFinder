using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OverLayApplicationSearch.Contract.Persistence.Entity;

namespace OverLayApplicationSearch.WpfApp.Contracts
{
    internal interface IListItemsViewModel
    {
        /// <summary>
        /// <see cref="ICommand"/> implementation for reloading the list items.
        /// </summary>
        ICommand ExecuteReloadItems { get; }

        /// <summary>
        /// <see cref="ICommand"/> implementation for deleting the <see cref="SelectedItem"/>.
        /// </summary>
        ICommand ExecuteDeleteItem { get; }

        /// <summary>
        /// <see cref="ICommand"/> implementation for synching <see cref="SelectedItem"/>.
        /// </summary>
        ICommand ExecuteSyncItem { get; }

        /// <summary>
        /// <see cref="ICommand"/> implementation for editing <see cref="SelectedItem"/>.
        /// </summary>
        ICommand ExecuteEditItem { get; }

        /// <summary>
        /// <see cref="ObservableCollection{T}"/> of tasks <see cref="IConfiguredTask"/>.
        /// </summary>
        ObservableCollection<IConfiguredTask> Items { get; }

        /// <summary>
        /// Selected <see cref="IConfiguredTask"/>.
        /// </summary>
        IConfiguredTask SelectedItem { get; set; }
    }
}
