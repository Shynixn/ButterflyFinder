using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OverLayApplicationSearch.WpfApp.Models
{
    internal class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Predicate<object> _predicate;

        public RelayCommand(Action<object> execute, Predicate<object> predicate)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            this._execute = execute;
            this._predicate = predicate;
        }

        public bool CanExecute(object parameter)
        {
            return _predicate == null || _predicate(parameter);
        }

        public void Execute(object parameter)
        {
            this._execute.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public static ICommand CreateCommand(ref ICommand command, Action<object> execute)
        {
            return CreateCommand(ref command, execute, null);
        }

        public static ICommand CreateCommand(ref ICommand command, Action<object> execute, Predicate<object> predicate)
        {
            if (command == null)
            {
                command = new RelayCommand(execute, predicate);
            }
            return command;
        }
    }
}
