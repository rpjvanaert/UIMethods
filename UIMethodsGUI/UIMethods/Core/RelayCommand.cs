using System;
using System.Windows.Input;

namespace UIMethods.Core
{
    internal class RelayCommand : ICommand
    {

        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove {  CommandManager.RequerySuggested -= value; }

        }

        public RelayCommand(Action<object> execute, Func<object, bool> callExecute = null)
        {
            _execute = execute;
            _canExecute = callExecute; 
        }

        public bool CanExecute(object paramter)
        {
            return _canExecute == null || _canExecute(paramter);
        }

        public void Execute(object paramter)
        {
            _execute(paramter);
        }
    }
}
