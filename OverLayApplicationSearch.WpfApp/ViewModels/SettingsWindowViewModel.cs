using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OverLayApplicationSearch.WpfApp.Contracts;
using OverLayApplicationSearch.WpfApp.Models;

namespace OverLayApplicationSearch.WpfApp.ViewModels
{
    internal class SettingsWindowViewModel : BaseViewModel<object>, ISettingsWindowViewModel
    {
        /// <summary>
        /// <see cref="ICommand"/> implementation for loading and showing the list page.
        /// </summary>
        public ICommand LoadListPage { get; }

        /// <summary>
        /// Element rendered in the sub grid of the settings window.
        /// </summary>
        public UIElementCollection ElementCollection { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="SettingsWindowViewModel"/>.
        /// </summary>
        public SettingsWindowViewModel()
        {
            LoadListPage = new RelayCommand(p => { LoadMainPageArea(typeof(ListItemsViewModel)); });
        }

        /// <summary>
        /// Changes the page rendered in the middle of the settings.
        /// </summary>
        /// <param name="typeOfModel">any viewModel type</param>
        public void LoadMainPageArea(Type typeOfModel)
        {
            var viewModel = typeOfModel.GetConstructor(new Type[] {typeof(ISettingsWindowViewModel)})
                .Invoke(new object[] {this});
            var anyType =
                Type.GetType(typeOfModel.FullName.Replace("ViewModels", "Views").Replace("ViewModel", "View"));
            var instance = Activator.CreateInstance(anyType);

            ((UserControl) instance).DataContext = viewModel;

            ElementCollection.Clear();
            ElementCollection.Add(instance as UIElement);
        }
    }
}