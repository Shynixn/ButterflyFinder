using System;
using System.Windows.Input;

namespace OverLayApplicationSearch.WpfApp.Models
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _predicate;

        /// <summary>
        /// Creates a new instance of <see cref="RelayCommand"/> with dependency <see cref="execute"/> and <see cref="predicate"/>.
        /// </summary>
        /// <param name="execute"><see cref="Action{T}"/></param>
        /// <param name="predicate"><see cref="Predicate{T}"/></param>
        public RelayCommand(Action<object> execute, Predicate<object> predicate)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            this._execute = execute;
            this._predicate = predicate;
        }

        /// <summary>
        /// Checks if the added action can be executed for the given <see cref="parameter"/>.
        /// </summary>
        /// <param name="parameter">any parameter</param>
        /// <returns>can execute</returns>
        public bool CanExecute(object parameter)
        {
            return _predicate == null || _predicate(parameter);
        }

        /// <summary>
        /// Executes the added action with the given <see cref="parameter"/>.
        /// </summary>
        /// <param name="parameter">any parameter</param>
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                this._execute.Invoke(parameter);
            }
        }

        /// <summary>
        /// Implementation.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="ICommand"/> if the given <see cref="command"/> is null and sets the 
        /// given <see cref="execute"/> action.
        /// </summary>
        /// <param name="command"><see cref="ICommand"/></param>
        /// <param name="execute"><see cref="Action{T}"/></param>
        /// <returns></returns>
        public static ICommand CreateCommand(ref ICommand command, Action<object> execute)
        {
            return CreateCommand(ref command, execute, null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ICommand"/> if the given <see cref="command"/> is null and sets the 
        /// given <see cref="execute"/> action with additional predicate..
        /// </summary>
        /// <param name="command"><see cref="ICommand"/></param>
        /// <param name="execute"><see cref="Action{T}"/></param>
        /// <param name="predicate"><see cref="Predicate{T}"/></param>
        /// <returns></returns>
        public static ICommand CreateCommand(ref ICommand command, Action<object> execute, Predicate<object> predicate)
        {
            return command ?? (command = new RelayCommand(execute, predicate));
        }
    }
}