using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace DiagramDesigner
{
    class MainViewModel 
    {
        public ICommand TestCommand { protected set; get; }

        public MainViewModel()
        {
            this.TestCommand = new DelegateCommand(ExecuteTestCommand);
        }
        private void ExecuteTestCommand(object obj)
        {
            System.Diagnostics.Debug.WriteLine("test running");
        }
    }
}
