using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.WpfApp.Contracts;

namespace OverLayApplicationSearch.WpfApp.Models
{
    internal class BaseViewModel<T> : INotifyPropertyChanged
    {
        private T model;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns the model of this viewModel.
        /// </summary>
        public T Model {
            get
            {
                if (model == null)
                {
                    model = Activator.CreateInstance<T>();
                }
                return model;
            } 
        }
    }
}
