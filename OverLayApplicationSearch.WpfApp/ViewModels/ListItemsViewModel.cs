using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.WpfApp.Contracts;
using OverLayApplicationSearch.WpfApp.Models;

namespace OverLayApplicationSearch.WpfApp.ViewModels
{
    internal class ListItemsViewModel : BaseViewModel<ListItemsModel>
    {
        /// <summary>
        /// Parent view Model implementing <see cref="ISettingsWindowViewModel"/>.
        /// </summary>
        public ISettingsWindowViewModel Parent { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ListItemsViewModel(ISettingsWindowViewModel parent)
        {
            Parent = parent;
       
            this.Model.DoSomething();
        }
    }
}