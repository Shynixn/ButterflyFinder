using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.WpfApp.Contracts;
using OverLayApplicationSearch.WpfApp.Models;

namespace OverLayApplicationSearch.WpfApp.ViewModels
{
    internal class ListItemsViewModel : BaseViewModel<ListItemsModel>, IListItemsViewModel
    {
        #region Public Properties

        /// <summary>
        /// Parent view Model implementing <see cref="ISettingsWindowViewModel"/>.
        /// </summary>
        public ISettingsWindowViewModel Parent { get; }

        /// <summary>
        /// <see cref="ObservableCollection{T}"/> of tasks <see cref="IConfiguredTask"/>.
        /// </summary>
        public ObservableCollection<IConfiguredTask> Items { get; private set; }

        /// <summary>
        /// Selected <see cref="IConfiguredTask"/>.
        /// </summary>
        public IConfiguredTask SelectedItem { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// <see cref="ICommand"/> implementation for reloading the list items.
        /// </summary>
        public ICommand ExecuteReloadItems { get; }

        /// <summary>
        /// <see cref="ICommand"/> implementation for deleting the <see cref="IListItemsViewModel.SelectedItem"/>.
        /// </summary>
        public ICommand ExecuteDeleteItem { get; }

        /// <summary>
        /// <see cref="ICommand"/> implementation for synching <see cref="IListItemsViewModel.SelectedItem"/>.
        /// </summary>
        public ICommand ExecuteSyncItem { get; }

        /// <summary>
        /// <see cref="ICommand"/> implementation for editing <see cref="IListItemsViewModel.SelectedItem"/>.
        /// </summary>
        public ICommand ExecuteEditItem { get; }

        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ListItemsViewModel(ISettingsWindowViewModel parent)
        {
            Parent = parent;

            this.ExecuteReloadItems = new RelayCommand(async o => { await RefreshItems(); });

              this.ExecuteReloadItems.Execute(null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Refreshes the <see cref="Items"/> from the <see cref="ListItemsModel"/>.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task RefreshItems()
        {
            await Task.Run(() => { Items = this.Model.GetAllTasks(); });
            OnPropertyChanged(nameof(Items));
        }

        /// <summary>
        /// Syncs the given <see cref="tasks"/> for the <see cref="ListItemsModel"/>
        /// </summary>
        /// <param name="tasks"><see cref="Collection{T}"/></param>
        /// <returns><see cref="Task"/></returns>
        private async Task SyncItems(Collection<IConfiguredTask> tasks)
        {
            await Task.Run(() => { Items = this.Model.GetAllTasks(); });

            // TODO: Write Code for Synching items
        }

        /// <summary>
        /// Deletes the given <see cref="tasks"/> from the <see cref="ListItemsModel"/>.
        /// </summary>
        /// <param name="tasks"><see cref="Collection{T}"/></param>
        /// <returns><see cref="Task"/></returns>
        private async Task DeleteItems(Collection<IConfiguredTask> tasks)
        {
            var result = MessageBox.Show("Are you sure you want to delete these task(s)? All associated indeces will be deleted and the files remain untouched.", "Delete",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            await Task.Run(() => { Model.DeleteTasks(tasks); });
            await RefreshItems();
        }

        #endregion
    }
}