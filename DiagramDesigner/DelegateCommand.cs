using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DiagramDesigner
{
    class DelegateCommand : ICommand
    {
        Action<object> execute;

        // required by ICommand
        public event EventHandler CanExecuteChanged;

        // constructor
        public DelegateCommand(Action<Object> execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            // TODO
            return true;
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
