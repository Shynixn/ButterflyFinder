using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace OverLayApplicationSearch.WpfApp.Contracts
{
    internal interface ISettingsWindowViewModel
    {
        /// <summary>
        /// <see cref="ICommand"/> implementation for loading and showing the list page.
        /// </summary>
        ICommand LoadListPage { get; }

        /// <summary>
        /// Element rendered in the sub grid of the settings window.
        /// </summary>
        UIElementCollection ElementCollection { get; set; }

        /// <summary>
        /// Changes the page rendered in the middle of the settings.
        /// </summary>
        /// <param name="typeOfModel">any viewModel type</param>
        void LoadMainPageArea(Type typeOfModel);
    }
}